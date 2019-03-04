using Authentication;
using GameServerServiceLayer;
using LobbyServiceLayer;
using LobbyServiceLayer.Photon;
using SinglePlayerServiceLayer;
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
	internal class KickStartSinglePlayerCommand : ICommand
	{
		private SinglePlayerLoadTDMAIParameters _singlePlayerAIParameters;

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

		public void Execute()
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Expected O, but got Unknown
			if (battleChecker.IsMachineValidForBattle())
			{
				loadingIconPresenter.NotifyLoading("GetCPULoadOnSinglePlayer");
				string mapName = worldSwitching.GetPracticeModePlanet();
				ISetParametersForSinglePlayerGameRequest setParametersForSinglePlayerGameRequest = lobbyRequestFactory.Create<ISetParametersForSinglePlayerGameRequest>();
				setParametersForSinglePlayerGameRequest.Inject(new SetParametersForSinglePlayerGameDependency(mapName));
				setParametersForSinglePlayerGameRequest.Execute();
				ParallelTaskCollection val = new ParallelTaskCollection();
				val.Add(ExecuteRequestAsTask<IGetGameModeSettingsRequest, GameModeSettingsDependency>(serviceRequestFactory, null, HandleServiceError));
				val.Add(ExecuteRequestAsTask<IGetScoreMultipliersTeamDeathMatchAIRequest, ScoreMultipliers>(serviceRequestFactory, null, HandleServiceError));
				val.Add(ExecuteRequestAsTask<IGetDefaultWeaponOrderSubcategoriesRequest, List<int>>(serviceRequestFactory, null, HandleServiceError));
				val.Add(ExecuteRequestAsTask<IGetPowerBarSettingsRequest, PowerBarSettingsData>(serviceRequestFactory, null, HandleServiceError));
				val.Add(ExecuteRequestAsTask<IGetAvatarInfoRequest, AvatarInfo>(serviceRequestFactory, HandleOnLocalAvatarInfoRetrieved, HandleServiceError));
				val.Add(ExecuteRequestAsTask<ILoadMachineRequest, LoadMachineResult>(serviceRequestFactory, null, HandleServiceError));
				val.Add(ExecuteRequestAsTask<IGetMyClanInfoRequest, ClanInfo>(socialRequestFactory, HandleOnClanInfoRetrieved, HandleServiceError));
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

		private IEnumerator ExecuteRequestAsTask<TRequestType, TReturnData>(IServiceRequestFactory requestFactory, Action<TReturnData> onComplete, Action<ServiceBehaviour> onFailed) where TRequestType : class, IServiceRequest, IAnswerOnComplete<TReturnData>
		{
			TaskService<TReturnData> taskService = requestFactory.Create<TRequestType>().AsTask();
			yield return taskService;
			if (!taskService.succeeded)
			{
				onFailed?.Invoke(taskService.behaviour);
			}
			else
			{
				onComplete?.Invoke(taskService.result);
			}
		}

		private IEnumerator GetAutoRegenSettingsRequestAsTask()
		{
			IGetAutoRegenHealthSettings request = serviceRequestFactory.Create<IGetAutoRegenHealthSettings>();
			request.ClearCache();
			TaskService<AutoRegenHealthSettingsData> taskService = request.AsTask();
			yield return taskService;
			if (!taskService.succeeded)
			{
				HandleServiceError(taskService.behaviour);
			}
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

		private IEnumerator BuildAndExecuteSinglePlayerRequest()
		{
			ISinglePlayerLoadTdmAiRobotsRequest loadAIRobots = singlePlayerRequestFactory.Create<ISinglePlayerLoadTdmAiRobotsRequest>();
			loadAIRobots.Inject(_singlePlayerAIParameters);
			loadAIRobots.SetAnswer(new ServiceAnswer<Dictionary<string, PlayerDataDependency>>(HandleBotsPlayerDataDependencyLoaded, HandleServiceError));
			yield return loadAIRobots;
		}

		private void HandleBotsPlayerDataDependencyLoaded(Dictionary<string, PlayerDataDependency> expectedPlayers)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			ISetExpectedPlayerRequest setExpectedPlayerRequest = lobbyRequestFactory.Create<ISetExpectedPlayerRequest>();
			setExpectedPlayerRequest.Inject(new ReadOnlyDictionary<string, PlayerDataDependency>(expectedPlayers));
			setExpectedPlayerRequest.Execute();
		}

		private void HandleLoadTasksComplete(string mapName)
		{
			SwitchWorldDependency dependency = new SwitchWorldDependency(mapName, isRanked_: false, isBrawl_: false, isCustomGame_: false, GameModeType.PraticeMode);
			commandFactory.Build<SwitchToSoloTdmPlanetCommand>().Inject(dependency).Execute();
			loadingIconPresenter.NotifyLoadingDone("GetCPULoadOnSinglePlayer");
			gUIInputController.CloseCurrentScreen();
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
		}

		private void HandleServiceError(ServiceBehaviour behaviour)
		{
			loadingIconPresenter.NotifyLoadingDone("GetCPULoadOnSinglePlayer");
			_loadDataTask.Stop();
			GenericErrorData error = new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, Localization.Get("strOK", true));
			ErrorWindow.ShowErrorWindow(error);
		}
	}
}
