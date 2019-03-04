using Authentication;
using GameServerServiceLayer;
using LobbyServiceLayer;
using LobbyServiceLayer.Photon;
using Mothership.RobotConfiguration;
using Services.Web.Photon;
using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign;
using SinglePlayerCampaign.GUI.Mothership;
using SinglePlayerServiceLayer;
using SinglePlayerServiceLayer.Requests.Photon;
using SocialServiceLayer;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class StartCampaignCommand : IInjectableCommand<SelectedCampaignToStart>, ICommand
	{
		private readonly Dictionary<string, PlayerDataDependency> _expectedPlayers = new Dictionary<string, PlayerDataDependency>();

		private SelectedCampaignToStart _startCampaignDependency;

		private SinglePlayerLoadTDMAIParameters _singlePlayerAIParameters;

		private LoadMachineResult _loadMachineResult;

		private byte[] _playerColorMap;

		private ITaskRoutine _loadDataTask;

		[Inject]
		public WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		public ISinglePlayerRequestFactory singlePlayerRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		public EnterBattleChecker battleChecker
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public ILobbyRequestFactory lobbyRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public IGUIInputControllerMothership gUIInputController
		{
			private get;
			set;
		}

		[Inject]
		public ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public IGUIInputController guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public GaragePresenter garagePresenter
		{
			private get;
			set;
		}

		[Inject]
		public RobotConfigurationDataSource robotConfigurationDataSource
		{
			private get;
			set;
		}

		public ICommand Inject(SelectedCampaignToStart dependency)
		{
			_startCampaignDependency = dependency;
			return this;
		}

		public void Execute()
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Expected O, but got Unknown
			if (battleChecker.IsMachineValidForBattle())
			{
				loadingIconPresenter.NotifyLoading("GetCPULoadOnSinglePlayer");
				string mapName = _startCampaignDependency.CampaignMap;
				ISetParametersForCampaignRequest setParametersForCampaignRequest = lobbyRequestFactory.Create<ISetParametersForCampaignRequest>();
				setParametersForCampaignRequest.Inject(new SetParametersForCampaignDependency(mapName));
				setParametersForCampaignRequest.Execute();
				ParallelTaskCollection val = new ParallelTaskCollection();
				val.Add(ExecuteRequestAsTask<IGetGameModeSettingsRequest, GameModeSettingsDependency>(serviceRequestFactory, null));
				val.Add(ExecuteRequestAsTask<IGetScoreMultipliersTeamDeathMatchAIRequest, ScoreMultipliers>(serviceRequestFactory, null));
				val.Add(ExecuteRequestAsTask<IGetDefaultWeaponOrderSubcategoriesRequest, List<int>>(serviceRequestFactory, null));
				val.Add(ExecuteRequestAsTask<IGetPowerBarSettingsRequest, PowerBarSettingsData>(serviceRequestFactory, null));
				val.Add(ExecuteRequestAsTask<IGetAvatarInfoRequest, AvatarInfo>(serviceRequestFactory, HandleOnLocalAvatarInfoRetrieved));
				val.Add(ExecuteRequestAsTask<ILoadMachineRequest, LoadMachineResult>(serviceRequestFactory, HumanPlayerMachineLoaded));
				val.Add(ExecuteRequestAsTask<IGetMyClanInfoRequest, ClanInfo>(socialRequestFactory, HandleOnClanInfoRetrieved));
				val.Add(GetAutoRegenSettingsRequestAsTask());
				SerialTaskCollection val2 = new SerialTaskCollection();
				val2.Add((IEnumerator)val);
				val2.Add(BuildAndExecuteSinglePlayerRequest());
				val2.add_onComplete((Action)delegate
				{
					HandleLoadTasksComplete(mapName);
				});
				_loadDataTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumerator((IEnumerator)val2);
				_loadDataTask.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private IEnumerator ExecuteRequestAsTask<TRequestType, TReturnData>(IServiceRequestFactory requestFactory, Action<TReturnData> onComplete) where TRequestType : class, IServiceRequest, IAnswerOnComplete<TReturnData>
		{
			TaskService<TReturnData> taskService = requestFactory.Create<TRequestType>().AsTask();
			yield return new HandleTaskServiceWithError(taskService, HandleServiceError, delegate
			{
			}).GetEnumerator();
			if (taskService.succeeded)
			{
				onComplete?.Invoke(taskService.result);
			}
		}

		private IEnumerator GetAutoRegenSettingsRequestAsTask()
		{
			IGetAutoRegenHealthSettings request = serviceRequestFactory.Create<IGetAutoRegenHealthSettings>();
			request.ClearCache();
			TaskService<AutoRegenHealthSettingsData> taskService = request.AsTask();
			yield return new HandleTaskServiceWithError(taskService, HandleServiceError, delegate
			{
			}).GetEnumerator();
		}

		private void HandleOnLocalAvatarInfoRetrieved(AvatarInfo localAvatarInfo)
		{
			_singlePlayerAIParameters.UsersPlayerAvatarInfo = new AvatarInfo(localAvatarInfo.UseCustomAvatar, localAvatarInfo.AvatarId);
		}

		private void HandleOnClanInfoRetrieved(ClanInfo myClanInfo)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			_singlePlayerAIParameters.UsersClanAvatarInfo = new AvatarInfo(useCustomAvatar: false, 0);
			Dictionary<string, ClanInfo> dictionary = new Dictionary<string, ClanInfo>();
			if (myClanInfo != null)
			{
				_singlePlayerAIParameters.localUserClan = myClanInfo.ClanName;
				_singlePlayerAIParameters.UsersClanAvatarInfo = new AvatarInfo(useCustomAvatar: false, myClanInfo.ClanAvatarNumber);
				dictionary[User.Username] = new ClanInfo(myClanInfo.ClanName, string.Empty, ClanType.Closed);
			}
			ISetClanInfosRequest setClanInfosRequest = lobbyRequestFactory.Create<ISetClanInfosRequest>();
			setClanInfosRequest.Inject(new ReadOnlyDictionary<string, ClanInfo>(dictionary));
			setClanInfosRequest.Execute();
		}

		private void HandleOnColorMapRetrieved(byte[] colorMap)
		{
			_playerColorMap = colorMap;
		}

		private IEnumerator BuildAndExecuteSinglePlayerRequest()
		{
			TaskService<byte[]> loadMachineColorMapTask = serviceRequestFactory.Create<ILoadMachineColorMapRequest, LoadMachineColorMapDependancy>(new LoadMachineColorMapDependancy(User.Username, garagePresenter.currentGarageSlot)).AsTask();
			yield return new HandleTaskServiceWithError(loadMachineColorMapTask, HandleServiceError, delegate
			{
			}).GetEnumerator();
			if (loadMachineColorMapTask.succeeded)
			{
				HandleOnColorMapRetrieved(loadMachineColorMapTask.result);
			}
			IGetCampaignWavesDataRequest getCampaignWavesDataRequest = singlePlayerRequestFactory.Create<IGetCampaignWavesDataRequest, GetCampaignWavesDependency>(new GetCampaignWavesDependency(_startCampaignDependency.CampaignID, _startCampaignDependency.Difficulty));
			getCampaignWavesDataRequest.ClearCache();
			TaskService<CampaignWavesDifficultyData> getCampaignWavesDataTask = getCampaignWavesDataRequest.AsTask();
			yield return new HandleTaskServiceWithError(getCampaignWavesDataTask, HandleServiceError, delegate
			{
			}).GetEnumerator();
			IGetGarageBayUniqueIdRequest getRobotUniqueIdRequest = singlePlayerRequestFactory.Create<IGetGarageBayUniqueIdRequest>();
			getRobotUniqueIdRequest.ClearCache();
			TaskService<UniqueSlotIdentifier> getRobotUniqueIdTask = getRobotUniqueIdRequest.AsTask();
			yield return new HandleTaskServiceWithError(getRobotUniqueIdTask, HandleServiceError, delegate
			{
			}).GetEnumerator();
			string dependency = getRobotUniqueIdTask.result.ToString();
			IGetRobotBayCustomisationsRequest getCustomisationsRequest = singlePlayerRequestFactory.Create<IGetRobotBayCustomisationsRequest, string>(dependency);
			getCustomisationsRequest.ClearCache();
			TaskService<GetRobotBayCustomisationsResponse> getCustomisationsTask = getCustomisationsRequest.AsTask();
			yield return new HandleTaskServiceWithError(getCustomisationsTask, HandleServiceError, delegate
			{
			}).GetEnumerator();
			if (getCampaignWavesDataTask.succeeded && getCustomisationsTask.succeeded)
			{
				string spawnEffect = robotConfigurationDataSource.GetSpawnEffect(getCustomisationsTask.result.SpawnEffectId);
				string deathEffect = robotConfigurationDataSource.GetDeathEffect(getCustomisationsTask.result.DeathEffectId);
				OnWavesDataReceived(getCampaignWavesDataTask.result, spawnEffect, deathEffect);
			}
		}

		private void OnWavesDataReceived(CampaignWavesDifficultyData campaignWavesDifficultyData, string spawnEffect_, string deathEffect_)
		{
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			string username = User.Username;
			string displayName = User.DisplayName;
			string currentRobotName = garagePresenter.CurrentRobotName;
			byte[] compresesdRobotData = _loadMachineResult.model.GetCompresesdRobotData();
			byte[] playerColorMap = _playerColorMap;
			PlayerDataDependency value = new PlayerDataDependency(username, displayName, currentRobotName, compresesdRobotData, 0u, hasPremium: false, garagePresenter.CurrentRobotIdentifier.ToString(), 0, _loadMachineResult.MasteryLevel, 0, _singlePlayerAIParameters.UsersPlayerAvatarInfo, _singlePlayerAIParameters.localUserClan, _singlePlayerAIParameters.UsersClanAvatarInfo, aiPlayer: false, spawnEffect_, deathEffect_, _loadMachineResult.weaponOrder.Serialise(), playerColorMap);
			_expectedPlayers.Add(User.Username, value);
			RobotNameHelper.ValidatePlayerName(User.Username);
			FasterList<WaveData> wavesData = campaignWavesDifficultyData.WavesData;
			WaveData waveData = wavesData.get_Item(0);
			WaveRobot[] waveRobots = waveData.WaveRobots;
			for (int i = 0; i < waveRobots.Length; i++)
			{
				for (int j = 0; j < waveRobots[i].maxRobotAmount; j++)
				{
					string @string = StringTableBase<StringTable>.Instance.GetString(waveRobots[i].robotName);
					string name = RobotNameHelper.GetName(waveRobots[i], i, j);
					currentRobotName = name;
					displayName = name;
					username = @string;
					playerColorMap = waveRobots[i].serializedRobotData;
					compresesdRobotData = waveRobots[i].serializedRobotDataColour;
					PlayerDataDependency value2 = new PlayerDataDependency(currentRobotName, displayName, username, playerColorMap, 1u, hasPremium: false, string.Empty, 0, 1, 0, new AvatarInfo(useCustomAvatar: false, 0), string.Empty, new AvatarInfo(useCustomAvatar: false, 0), aiPlayer: true, "Spawn", "Explosion", new int[0], compresesdRobotData);
					_expectedPlayers.Add(name, value2);
				}
			}
			ISetExpectedPlayerRequest setExpectedPlayerRequest = lobbyRequestFactory.Create<ISetExpectedPlayerRequest>();
			setExpectedPlayerRequest.Inject(new ReadOnlyDictionary<string, PlayerDataDependency>(_expectedPlayers));
			setExpectedPlayerRequest.Execute();
		}

		private void HumanPlayerMachineLoaded(LoadMachineResult loadMachineResult)
		{
			_loadMachineResult = loadMachineResult;
		}

		private void HandleLoadTasksComplete(string mapName)
		{
			SwitchSinglePlayerCampaignWorldDependency dependency = new SwitchSinglePlayerCampaignWorldDependency(mapName, _startCampaignDependency.CampaignID, _startCampaignDependency.Difficulty, _startCampaignDependency.CampaignName);
			commandFactory.Build<SwitchToCampaignCommand>().Inject(dependency).Execute();
			loadingIconPresenter.NotifyLoadingDone("GetCPULoadOnSinglePlayer");
			gUIInputController.CloseCurrentScreen();
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
		}

		private void HandleServiceError()
		{
			loadingIconPresenter.NotifyLoadingDone("GetCPULoadOnSinglePlayer");
			_loadDataTask.Stop();
		}
	}
}
