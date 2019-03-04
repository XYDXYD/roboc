using Authentication;
using Battle;
using LobbyServiceLayer;
using RCNetwork.Client.UNet;
using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign;
using SinglePlayerCampaign.Simulation.EntityViews;
using SinglePlayerServiceLayer;
using SinglePlayerServiceLayer.Requests.Photon;
using Svelto.Command;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Simulation.SinglePlayerCampaign
{
	internal sealed class CampaignMachineLoaderEngine : SingleEntityViewEngine<CurrentWaveTrackerEntityView>, IInitialize, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private int _currentWave;

		private CurrentWaveTrackerEntityView _currentWaveTrackerEntityView;

		private CampaignWavesDifficultyData _campaignWavesDifficultyData;

		private PlayerDataDependency _humanPlayerData;

		private ReadOnlyDictionary<string, PlayerDataDependency> _expectedPlayers;

		private readonly NetworkInitialisationMockClientUnity _networkInitialisationMockClientUnity;

		[Inject]
		internal MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer _playerNamesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		[Inject]
		internal IInitialSimulationGUIFlow initialSingleplayerGUIFlowCampaign
		{
			private get;
			set;
		}

		[Inject]
		internal ILobbyRequestFactory lobbyRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISinglePlayerRequestFactory singlePlayerRequestFactory
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public CampaignMachineLoaderEngine(NetworkInitialisationMockClientUnity networkInitialisationMockClientUnity)
		{
			_networkInitialisationMockClientUnity = networkInitialisationMockClientUnity;
		}

		void IQueryingEntityViewEngine.Ready()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			initialSingleplayerGUIFlowCampaign.OnGuiFlowComplete += HandleOnGuiFlowComplete;
			IGetCampaignWavesDataRequest getCampaignWavesDataRequest = singlePlayerRequestFactory.Create<IGetCampaignWavesDataRequest>();
			getCampaignWavesDataRequest.Inject(new GetCampaignWavesDependency(WorldSwitching.GetCampaignID(), WorldSwitching.GetCampaignDifficulty()));
			getCampaignWavesDataRequest.SetAnswer(new ServiceAnswer<CampaignWavesDifficultyData>(OnWavesDataReceived));
			getCampaignWavesDataRequest.Execute();
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			initialSingleplayerGUIFlowCampaign.OnGuiFlowComplete -= HandleOnGuiFlowComplete;
		}

		private void HandleOnGuiFlowComplete()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadRobotsForTheFirstWave);
		}

		protected override void Add(CurrentWaveTrackerEntityView entityView)
		{
			_currentWaveTrackerEntityView = entityView;
			entityView.CurrentWaveCounterComponent.counterValue.NotifyOnValueSet((Action<int, int>)HandleNewWave);
		}

		protected override void Remove(CurrentWaveTrackerEntityView entityView)
		{
			entityView.CurrentWaveCounterComponent.counterValue.StopNotify((Action<int, int>)HandleNewWave);
		}

		private void HandleNewWave(int entityID, int currentWave)
		{
			int maxValue = entityViewsDB.QueryEntityView<CurrentWaveTrackerEntityView>(entityID).CurrentWaveCounterComponent.maxValue;
			if (currentWave > 0 && currentWave < maxValue)
			{
				_currentWave = currentWave;
				lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>>(HandleExpectedPlayersRetrieved)).Execute();
			}
		}

		private IEnumerator LoadRobotsForTheFirstWave()
		{
			yield return LoadRobots();
			commandFactory.Build<StartCampaignGameCommand>().Execute();
		}

		private IEnumerator LoadRobots()
		{
			yield return PreloadMachines();
			int localId = _playerNamesContainer.GetPlayerId(User.Username);
			if (_currentWave == 0)
			{
				_networkInitialisationMockClientUnity.Start(localId);
			}
			_currentWaveTrackerEntityView.ReadyToSpawnWaveComponent.ReadyToSpawn.set_value(true);
			_currentWaveTrackerEntityView.ReadyToSpawnWaveComponent.ReadyToSpawn.set_value(false);
		}

		private IEnumerator PreloadMachines()
		{
			commandFactory.Build<GeneratePlayerIDsMockClientCommand>().Execute();
			InitialMultiplayerGUIFlow.LoadPlayerTeams(battlePlayers, playerTeamsContainer, _playerNamesContainer);
			List<PlayerDataDependency> expectedPlayersList = battlePlayers.GetExpectedPlayersList();
			List<int> list = new List<int>();
			foreach (PlayerDataDependency item in expectedPlayersList)
			{
				if (item.AiPlayer)
				{
					list.Add(_playerNamesContainer.GetPlayerId(item.PlayerName));
				}
			}
			PlayerIDsDependency dependency = new PlayerIDsDependency(list.ToArray());
			commandFactory.Build<SetHostedAIsClientCommand>().Inject(dependency).Execute();
			if (_currentWave > 0)
			{
				yield return machinePreloader.PreloadAllEnemyMachines();
			}
			else
			{
				yield return machinePreloader.PreloadAllMachines();
			}
		}

		private void OnWavesDataReceived(CampaignWavesDifficultyData campaignWavesDifficultyData)
		{
			_campaignWavesDifficultyData = campaignWavesDifficultyData;
		}

		private void HandleExpectedPlayersRetrieved(ReadOnlyDictionary<string, PlayerDataDependency> expectedPlayers)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_expectedPlayers = expectedPlayers;
			_humanPlayerData = _expectedPlayers.get_Item(User.Username);
			PrepareNewWave();
		}

		private void PrepareNewWave()
		{
			//IL_015a: Unknown result type (might be due to invalid IL or missing references)
			Dictionary<string, PlayerDataDependency> dictionary = new Dictionary<string, PlayerDataDependency>();
			dictionary.Add(User.Username, _humanPlayerData);
			Dictionary<string, PlayerDataDependency> dictionary2 = dictionary;
			RobotNameHelper.ValidatePlayerName(User.Username);
			FasterList<WaveData> wavesData = _campaignWavesDifficultyData.WavesData;
			WaveData waveData = wavesData.get_Item(_currentWave);
			WaveRobot[] waveRobots = waveData.WaveRobots;
			for (int i = 0; i < waveRobots.Length; i++)
			{
				for (int j = 0; j < waveRobots[i].maxRobotAmount; j++)
				{
					string @string = StringTableBase<StringTable>.Instance.GetString(waveRobots[i].robotName);
					string name = RobotNameHelper.GetName(waveRobots[i], i, j);
					string playerName = name;
					string displayName = name;
					string robotName = @string;
					byte[] serializedRobotData = waveRobots[i].serializedRobotData;
					byte[] serializedRobotDataColour = waveRobots[i].serializedRobotDataColour;
					PlayerDataDependency value = new PlayerDataDependency(playerName, displayName, robotName, serializedRobotData, 1u, hasPremium: false, string.Empty, 0, 1, 0, new AvatarInfo(useCustomAvatar: false, 0), string.Empty, new AvatarInfo(useCustomAvatar: false, 0), aiPlayer: true, "Spawn", "Explosion", new int[0], serializedRobotDataColour);
					dictionary2.Add(name, value);
				}
			}
			InitialMultiplayerGUIFlow.ClearPlayerTeams(battlePlayers, playerTeamsContainer, _playerNamesContainer);
			commandFactory.Build<ClearPlayerIDsMockClientCommand>().Execute();
			ISetExpectedPlayerRequest setExpectedPlayerRequest = lobbyRequestFactory.Create<ISetExpectedPlayerRequest>();
			setExpectedPlayerRequest.Inject(new ReadOnlyDictionary<string, PlayerDataDependency>(dictionary2));
			setExpectedPlayerRequest.Execute();
			lobbyRequestFactory.Create<IRetrieveExpectedPlayersListRequest>().SetAnswer(new ServiceAnswer<ReadOnlyDictionary<string, PlayerDataDependency>>(delegate(ReadOnlyDictionary<string, PlayerDataDependency> newExpectedPlayers)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				battlePlayers.SetExpectedPlayers(newExpectedPlayers);
				TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadRobots);
			})).Execute();
		}
	}
}
