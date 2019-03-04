using Battle;
using InputMask;
using Mothership.GUI.Social;
using RCNetwork.Client.UNet;
using RCNetwork.Events;
using RCNetwork.Server;
using Robocraft.GUI;
using Simulation;
using Simulation.Battle;
using Simulation.Hardware.Modules;
using Simulation.SinglePlayer;
using Simulation.SinglePlayer.Rewards;
using Simulation.SinglePlayer.ServerMock;
using Simulation.SinglePlayer.Spawn;
using Simulation.SinglePlayer.StunModule;
using SinglePlayerServiceLayer;
using Svelto.ECS;
using Svelto.ServiceLayer;
using UnityEngine;
using WebServices;

internal class MainSimulationTeamDeathMatchModeAI : MainSimulation
{
	private BattleStatsPresenter _battleStatsPresenter;

	private GameStartAudioTrigger _gameStartAudioTrigger;

	private PlayerSpawnPointObservable _playerSpawnPointObservable;

	private BattleStatsEngine _battleStatsEngine;

	private TDMGameStartAudio _tdmGameStartAudio;

	private GameStartAndEndTracker _gameStartAndEndTrackerTencent;

	public override void SetUpContainerForContext()
	{
		base.container.Bind<IServiceRequestFactory>().AsSingle<WebStorageRequestFactoryDefault>();
		base.container.Bind<ISinglePlayerRequestFactory>().AsSingle<SinglePlayerRequestFactory>();
		base.container.Bind<BattleParameters>().AsSingle<BattleParametersSinglePlayer>();
		base.container.Bind<IPauseMenuController>().AsSingle<PauseMenuControllerPracticeMode>();
		base.container.Bind<IBattleEventStreamManager>().AsSingle<BattleEventStreamManager>();
		base.container.BindSelf<BattleStatsPresenter>();
		base.container.Bind<ISpectatorModeActivator>().AsSingle<SinglePlayerSpectatorModeActivator>();
		base.container.BindSelf<SpectatorHintPresenter>();
		base.container.BindSelf<ReadyToRespawnPresenter>();
		base.container.BindSelf<SurrenderControllerClient>();
		base.container.Bind<IInputActionMask>().AsSingle<InputActionMaskNormal>();
		base.container.Bind<IInitialSimulationGUIFlow>().AsInstance<InitialSingleplayerGUIFlow>(new InitialSingleplayerGUIFlow(new NetworkInitialisationMockClientUnity(base.container, isTestMode: false)));
		base.container.BindSelf<ShowMapComponent>();
		base.container.BindSelf<SpawnMapPingComponent>();
		base.container.BindSelf<MapPingCooldownObserver>();
		base.container.BindSelf<MapPingClientCommandObserver>();
		base.container.BindSelf<MapPingCreationObserver>();
		base.container.BindSelf<PingIndicatorCreationObserver>();
		base.container.BindSelf<RoboPointAwardManager>();
		base.container.BindSelf<HealCubesBonusManager>();
		base.container.BindSelf<ProtectTeamMateBonusManager>();
		base.container.BindSelf<DefendTheBaseBonusManager>();
		base.container.BindSelf<TeamBaseProgressDispatcher>();
		base.container.BindSelf<TeamBaseAudioGui>();
		base.container.BindSelf<NetworkInputRecievedManager>();
		base.container.BindSelf<ClientPing>();
		base.container.BindSelf<SendDisconnectToServerOnWorldSwitch>();
		base.container.Bind<ChatWarningDialogue>().AsInstance<ChatWarningDialogue>(new ChatWarningDialogue());
		base.container.BindSelf<RobotNameWriterPresenter>();
		base.container.BindSelf<TeamDeathMatchStatsPresenter>();
		base.container.BindSelf<RespawnHealEffects>();
		base.container.BindSelf<RespawnHealthSettingsObserver>();
		base.container.BindSelf<StatsUpdatedObservable>();
		base.container.BindSelf<FinalStatsUpdatedObservable>();
		base.container.BindSelf<TDMPracticeGamEndedObservable>();
		base.container.Bind<IComponentFactory>().AsSingle<GenericComponentFactory>();
		base.container.Bind<IMinimapPresenter>().AsSingle<MinimapPresenter>();
		base.container.Bind<IMusicManager>().AsSingle<TDMMusicManager>();
		base.container.BindSelf<TDMMusicManager>();
		SinglePlayerSpecificBindings();
	}

	public override void SetupNetworkContainerForContext()
	{
		base.container.Bind<INetworkEventManagerClient>().AsSingle<NetworkEventManagerClientMock>();
		base.container.Bind<INetworkEventManagerServer>().AsSingle<NetworkEventManagerServerMock>();
		base.container.Bind<ISpawnPointManager>().AsSingle<SinglePlayerSpawnPointManager>();
		base.container.Bind<IServerTimeClient>().AsSingle<ServerTimeClientMock>();
		base.container.Bind<ILobbyPlayerListPresenter>().AsSingle<LobbyPlayerListPresenter>();
		base.container.BindSelf<BonusManager>();
		base.container.BindSelf<MultiplayerAvatars>();
	}

	public override void SetupTickablesForContext()
	{
		_tickEngine.Add(base.container.Build<ClientPing>());
	}

	public override void SetUpEntitySystemForContext()
	{
		SpawnPointsEngine spawnPointsEngine = base.container.Inject<SpawnPointsEngine>(new SpawnPointsEngine(new PlayerSpawnPointObserver(_playerSpawnPointObservable)));
		_nodeEnginesRoot.AddEngine(spawnPointsEngine);
		StatsUpdatedObservable observable = base.container.Build<StatsUpdatedObservable>();
		FinalStatsUpdatedObservable observable2 = base.container.Build<FinalStatsUpdatedObservable>();
		TDMPracticeGamEndedObservable tdmPracticeGamEndedObservable = base.container.Build<TDMPracticeGamEndedObservable>();
		StatsUpdatedObserver statsUpdatedObserver = new StatsUpdatedObserver(observable);
		FinalStatsUpdatedObserver finalStatsUpdatedObserver = new FinalStatsUpdatedObserver(observable2);
		TDMPracticeGamEndedObserver tdmPracticeGamEndedObserver = new TDMPracticeGamEndedObserver(tdmPracticeGamEndedObservable);
		SinglePlayerRewardsEngine singlePlayerRewardsEngine = base.container.Inject<SinglePlayerRewardsEngine>(new SinglePlayerRewardsEngine(statsUpdatedObserver, finalStatsUpdatedObserver, tdmPracticeGamEndedObserver));
		_nodeEnginesRoot.AddEngine(singlePlayerRewardsEngine);
		_nodeEnginesRoot.AddEngine(_battleStatsEngine);
		AllPlayersSpawnedObservable observable3 = new AllPlayersSpawnedObservable();
		AllPlayersSpawnedObserver observer = new AllPlayersSpawnedObserver(observable3);
		AISpawner aISpawner = base.container.Inject<AISpawner>(new AISpawner(observable3));
		_nodeEnginesRoot.AddEngine(aISpawner);
		_contextNotifier.AddFrameworkDestructionListener(aISpawner);
		SinglePlayerTeamDeathMatchServerMock singlePlayerTeamDeathMatchServerMock = base.container.Inject<SinglePlayerTeamDeathMatchServerMock>(new SinglePlayerTeamDeathMatchServerMock(observer));
		_nodeEnginesRoot.AddEngine(singlePlayerTeamDeathMatchServerMock);
		_tickEngine.Add(singlePlayerTeamDeathMatchServerMock);
		AIStunEngine aIStunEngine = base.container.Inject<AIStunEngine>(new AIStunEngine());
		_nodeEnginesRoot.AddEngine(aIStunEngine);
		NetworkStunMachineObserver observer2 = new NetworkStunMachineObserver(_networkStunMachineObservable);
		EmpModuleStunEngine empModuleStunEngine = base.container.Inject<EmpModuleStunEngine>(new EmpModuleStunEngine(isSinglePlayer: true, observer2));
		_nodeEnginesRoot.AddEngine(empModuleStunEngine);
		_contextNotifier.AddFrameworkDestructionListener(empModuleStunEngine);
		TLOG_PlayerKillTrackerDataCacheFactory_Tencent tLOG_PlayerKillTrackerDataCacheFactory_Tencent = new TLOG_PlayerKillTrackerDataCacheFactory_Tencent(base.container.Build<IEntityFactory>(), base.container, _nodeEnginesRoot);
		tLOG_PlayerKillTrackerDataCacheFactory_Tencent.BuildEntities();
		tLOG_PlayerKillTrackerDataCacheFactory_Tencent.BuildEngine();
		TLOG_AIPlayerKillTrackerEngineTracker_Tencent tLOG_AIPlayerKillTrackerEngineTracker_Tencent = base.container.Inject<TLOG_AIPlayerKillTrackerEngineTracker_Tencent>(new TLOG_AIPlayerKillTrackerEngineTracker_Tencent());
		_nodeEnginesRoot.AddEngine(tLOG_AIPlayerKillTrackerEngineTracker_Tencent);
	}

	public override void SetUpEntitySystemForContextLegacy()
	{
		_battleStatsEngine = base.container.Inject<BattleStatsEngine>(new BattleStatsEngine());
		base.enginesRoot.AddEngine(_battleStatsEngine);
		_battleStatsPresenter = base.container.Build<BattleStatsPresenter>();
		base.enginesRoot.AddComponent(_battleStatsPresenter);
		_guiInputController.AddDisplayScreens(new IGUIDisplay[5]
		{
			base.container.Build<IPauseMenuController>() as IGUIDisplay,
			base.container.Build<ControlsDisplay>(),
			base.container.Build<SettingsDisplay>(),
			base.container.Build<GenericInfoDisplay>(),
			_battleStatsPresenter
		});
		MapPingEngine engine = base.container.Inject<MapPingEngine>(new MapPingEngine());
		base.enginesRoot.AddEngine(engine);
		PingObjectsManagementEngine pingObjectsManagementEngine = base.container.Inject<PingObjectsManagementEngine>(new PingObjectsManagementEngine());
		base.enginesRoot.AddEngine(pingObjectsManagementEngine);
		_tickEngine.Add(pingObjectsManagementEngine);
		MapPingManagementEngine mapPingManagementEngine = base.container.Inject<MapPingManagementEngine>(new MapPingManagementEngine());
		base.enginesRoot.AddEngine(mapPingManagementEngine);
		_tickEngine.Add(mapPingManagementEngine);
		PingIndicatorEngine engine2 = base.container.Inject<PingIndicatorEngine>(new PingIndicatorEngine());
		base.enginesRoot.AddEngine(engine2);
		CameraPingIndicatorEngine cameraPingIndicatorEngine = base.container.Inject<CameraPingIndicatorEngine>(new CameraPingIndicatorEngine());
		base.enginesRoot.AddEngine(cameraPingIndicatorEngine);
		_tickEngine.Add(cameraPingIndicatorEngine);
		ShowMapEngine showMapEngine = base.container.Inject<ShowMapEngine>(new ShowMapEngine());
		base.enginesRoot.AddEngine(showMapEngine);
		_tickEngine.Add(showMapEngine);
		PingTypeSelectionEngine pingTypeSelectionEngine = new PingTypeSelectionEngine();
		base.enginesRoot.AddEngine(pingTypeSelectionEngine);
		_tickEngine.Add(pingTypeSelectionEngine);
		PingSelectorEngine pingSelectorEngine = new PingSelectorEngine();
		base.enginesRoot.AddEngine(pingSelectorEngine);
		_tickEngine.Add(pingSelectorEngine);
		base.container.Inject<DisconnectedPlayerVoiceOver>(new DisconnectedPlayerVoiceOver());
		ShowMapComponent component = base.container.Build<ShowMapComponent>();
		base.enginesRoot.AddComponent(component);
		IPingObjectsManagementComponent component2 = base.container.Build<IPingObjectsManagementComponent>();
		base.enginesRoot.AddComponent(component2);
		IBattleStatsModeComponent component3 = base.container.Build<IBattleStatsModeComponent>();
		base.enginesRoot.AddComponent(component3);
		IBattleStatsInputComponent component4 = base.container.Build<IBattleStatsInputComponent>();
		base.enginesRoot.AddComponent(component4);
		IMapModeComponent component5 = base.container.Build<IMapModeComponent>();
		base.enginesRoot.AddComponent(component5);
		_gameStartAudioTrigger = base.container.Inject<GameStartAudioTrigger>(new GameStartAudioTrigger());
		_contextNotifier.AddFrameworkInitializationListener(_gameStartAudioTrigger);
		_contextNotifier.AddFrameworkDestructionListener(_gameStartAudioTrigger);
		SinglePlayerSetupContextLegacy();
	}

	public override void BuildGameObjects()
	{
		_gameObjectFactory.Build("HUD_TeamDeathmatch5v5");
		_gameObjectFactory.Build("HUD_Players");
		GameObject gameObject = _gameObjectFactory.Build("SocialPanel_Multiplayer").get_transform().GetChild(0)
			.get_gameObject();
		SocialGUIFactory socialGUIFactory = base.container.Inject<SocialGUIFactory>(new SocialGUIFactory());
		socialGUIFactory.Build(gameObject, base.container);
	}

	public override void BuildClassesForContext()
	{
		base.container.Build<RoboPointAwardManager>();
		base.container.Build<TeamDeathMatchAIScoreServerMock>();
		base.container.Build<BonusManager>();
		base.container.Build<TeamBaseAudioGui>();
		base.container.Build<IInitialSimulationGUIFlow>();
		base.container.Build<SendDisconnectToServerOnWorldSwitch>();
		_tdmGameStartAudio = base.container.Inject<TDMGameStartAudio>(new TDMGameStartAudio());
		_gameStartAndEndTrackerTencent = base.container.Inject<GameStartAndEndTracker>(new GameStartAndEndTracker());
	}

	private void SinglePlayerSpecificBindings()
	{
		base.container.BindSelf<AIPreloadRobotBuilder>();
		base.container.Bind<SinglePlayerRespawner>().AsSingle<SinglePlayerTeamDeathMatchRespawner>();
		base.container.Bind<BattleParameters>().AsSingle<BattleParametersSinglePlayer>();
		base.container.Bind<SinglePlayerWeaponFireValidator>().AsSingle<TDMPracticeWeaponFireValidator>();
		base.container.Bind<ChatPresenter>().AsSingle<ChatPresenterSinglePlayer>();
		base.container.BindSelf<ChatChannelCommands>();
		_playerSpawnPointObservable = new PlayerSpawnPointObservable();
		base.container.Bind<PlayerSpawnPointObservable>().AsInstance<PlayerSpawnPointObservable>(_playerSpawnPointObservable);
		base.container.BindSelf<TeamDeathMatchAIScoreServerMock>();
	}

	private void SinglePlayerSetupContextLegacy()
	{
		SinglePlayerWeaponFireValidator component = base.container.Build<SinglePlayerWeaponFireValidator>();
		base.enginesRoot.AddComponent(component);
	}
}
