using Achievements;
using Battle;
using InputMask;
using LobbyServiceLayer;
using RCNetwork.Client.UNet;
using RCNetwork.Events;
using RCNetwork.Server;
using Robocraft.GUI;
using Services.Analytics;
using Simulation;
using Simulation.Battle;
using Simulation.GUI;
using Simulation.Hardware.Modules;
using Simulation.SinglePlayer;
using Simulation.SinglePlayer.Rewards;
using Simulation.SinglePlayer.ServerMock;
using Simulation.SinglePlayer.Spawn;
using Simulation.SinglePlayer.StunModule;
using Simulation.SinglePlayerCampaign;
using SinglePlayerCampaign.GUI;
using SinglePlayerCampaign.GUI.Simulation.Engines;
using SinglePlayerCampaign.Simulation;
using SinglePlayerCampaign.Simulation.Engines;
using SinglePlayerServiceLayer;
using Svelto.Command;
using Svelto.ECS;
using Svelto.ServiceLayer;
using UnityEngine;
using WebServices;

internal class MainSimulationCampaign : MainSimulation
{
	private BattleStatsPresenter _battleStatsPresenter;

	private GameStartAudioTrigger _gameStartAudioTrigger;

	private PlayerSpawnPointObservable _playerSpawnPointObservable;

	private CurrentWaveObservable _currentWaveObservable;

	private GameStartAndEndTracker _gameStartAndEndTrackerTencent;

	private NetworkInitialisationMockClientUnity _networkInitialisation;

	public override void SetUpContainerForContext()
	{
		base.container.Bind<IServiceRequestFactory>().AsSingle<WebStorageRequestFactoryCampaign>();
		base.container.Bind<ISinglePlayerRequestFactory>().AsSingle<SinglePlayerRequestFactory>();
		base.container.Bind<BattleParameters>().AsSingle<BattleParametersSinglePlayer>();
		base.container.Bind<IPauseMenuController>().AsSingle<PauseMenuControllerPracticeMode>();
		base.container.Bind<IBattleEventStreamManager>().AsSingle<BattleEventStreamManager>();
		base.container.BindSelf<BattleStatsPresenter>();
		base.container.Bind<ISpectatorModeActivator>().AsSingle<SinglePlayerSpectatorModeActivator>();
		base.container.BindSelf<RoboPointAwardManager>();
		base.container.BindSelf<HealCubesBonusManager>();
		base.container.BindSelf<ProtectTeamMateBonusManager>();
		base.container.BindSelf<DefendTheBaseBonusManager>();
		base.container.BindSelf<SpectatorHintPresenter>();
		base.container.BindSelf<ReadyToRespawnPresenter>();
		base.container.BindSelf<SurrenderControllerClient>();
		base.container.Bind<IInputActionMask>().AsSingle<InputActionMaskNormal>();
		base.container.Bind<IInitialSimulationGUIFlow>().AsInstance<InitialSingleplayerGUIFlowCampaign>(new InitialSingleplayerGUIFlowCampaign());
		base.container.BindSelf<ShowMapComponent>();
		base.container.BindSelf<SpawnMapPingComponent>();
		base.container.BindSelf<MapPingCooldownObserver>();
		base.container.BindSelf<MapPingClientCommandObserver>();
		base.container.BindSelf<MapPingCreationObserver>();
		base.container.BindSelf<PingIndicatorCreationObserver>();
		base.container.BindSelf<TeamBaseProgressDispatcher>();
		base.container.BindSelf<TeamBaseAudioGui>();
		base.container.BindSelf<NetworkInputRecievedManager>();
		base.container.BindSelf<ClientPing>();
		base.container.BindSelf<SendDisconnectToServerOnWorldSwitch>();
		base.container.BindSelf<RobotNameWriterPresenter>();
		base.container.BindSelf<RespawnHealEffects>();
		base.container.BindSelf<RespawnHealthSettingsObserver>();
		base.container.BindSelf<StatsUpdatedObservable>();
		base.container.BindSelf<FinalStatsUpdatedObservable>();
		base.container.BindSelf<TDMPracticeGamEndedObservable>();
		base.container.BindSelf<CurrentWaveObservable>();
		_currentWaveObservable = base.container.Build<CurrentWaveObservable>();
		base.container.Bind<CurrentWaveObserver>().AsInstance<CurrentWaveObserver>(new CurrentWaveObserver(_currentWaveObservable));
		base.container.Bind<IComponentFactory>().AsSingle<GenericComponentFactory>();
		base.container.Bind<IMinimapPresenter>().AsSingle<MinimapPresenter>();
		base.container.Bind<IMusicManager>().AsSingle<SPCampaignMusicManager>();
		base.container.BindSelf<SPCampaignMusicManager>();
		SinglePlayerSpecificBindings();
	}

	public override void SetupNetworkContainerForContext()
	{
		_networkInitialisation = new NetworkInitialisationMockClientUnity(base.container, isTestMode: false);
		base.container.Bind<INetworkEventManagerClient>().AsSingle<NetworkEventManagerClientMock>();
		base.container.Bind<INetworkEventManagerServer>().AsSingle<NetworkEventManagerServerMock>();
		base.container.Bind<ISpawnPointManager>().AsSingle<SinglePlayerCampaignSpawnPointManager>();
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
		StatsUpdatedObservable observable = base.container.Build<StatsUpdatedObservable>();
		FinalStatsUpdatedObservable observable2 = base.container.Build<FinalStatsUpdatedObservable>();
		TDMPracticeGamEndedObservable tdmPracticeGamEndedObservable = base.container.Build<TDMPracticeGamEndedObservable>();
		StatsUpdatedObserver statsUpdatedObserver = new StatsUpdatedObserver(observable);
		FinalStatsUpdatedObserver finalStatsUpdatedObserver = new FinalStatsUpdatedObserver(observable2);
		TDMPracticeGamEndedObserver tdmPracticeGamEndedObserver = new TDMPracticeGamEndedObserver(tdmPracticeGamEndedObservable);
		SinglePlayerRewardsEngine singlePlayerRewardsEngine = base.container.Inject<SinglePlayerRewardsEngine>(new SinglePlayerRewardsEngine(statsUpdatedObserver, finalStatsUpdatedObserver, tdmPracticeGamEndedObserver));
		_nodeEnginesRoot.AddEngine(singlePlayerRewardsEngine);
		AllPlayersSpawnedObservable allPlayersSpawnedObservable = new AllPlayersSpawnedObservable();
		AllPlayersSpawnedObserver allPlayersSpawnedObserver = new AllPlayersSpawnedObserver(allPlayersSpawnedObservable);
		CampaignMachineLoaderEngine campaignMachineLoaderEngine = base.container.Inject<CampaignMachineLoaderEngine>(new CampaignMachineLoaderEngine(_networkInitialisation));
		_nodeEnginesRoot.AddEngine(campaignMachineLoaderEngine);
		CampaignSpawnPointsEngine campaignSpawnPointsEngine = base.container.Inject<CampaignSpawnPointsEngine>(new CampaignSpawnPointsEngine(new PlayerSpawnPointObserver(_playerSpawnPointObservable), allPlayersSpawnedObservable, base.container.Build<ICursorMode>(), _allowMovementObservable));
		_nodeEnginesRoot.AddEngine(campaignSpawnPointsEngine);
		_contextNotifier.AddFrameworkDestructionListener(campaignSpawnPointsEngine);
		CampaignServerMock campaignServerMock = base.container.Inject<CampaignServerMock>(new CampaignServerMock(allPlayersSpawnedObserver, base.container.Build<IAchievementManager>()));
		_nodeEnginesRoot.AddEngine(campaignServerMock);
		AIStunEngine aIStunEngine = base.container.Inject<AIStunEngine>(new AIStunEngine());
		_nodeEnginesRoot.AddEngine(aIStunEngine);
		NetworkStunMachineObserver observer = new NetworkStunMachineObserver(_networkStunMachineObservable);
		EmpModuleStunEngine empModuleStunEngine = base.container.Inject<EmpModuleStunEngine>(new EmpModuleStunEngine(isSinglePlayer: true, observer));
		_nodeEnginesRoot.AddEngine(empModuleStunEngine);
		_contextNotifier.AddFrameworkDestructionListener(empModuleStunEngine);
		TLOG_PlayerKillTrackerDataCacheFactory_Tencent tLOG_PlayerKillTrackerDataCacheFactory_Tencent = new TLOG_PlayerKillTrackerDataCacheFactory_Tencent(base.container.Build<IEntityFactory>(), base.container, _nodeEnginesRoot);
		tLOG_PlayerKillTrackerDataCacheFactory_Tencent.BuildEntities();
		tLOG_PlayerKillTrackerDataCacheFactory_Tencent.BuildEngine();
		TLOG_AIPlayerKillTrackerEngineTracker_Tencent tLOG_AIPlayerKillTrackerEngineTracker_Tencent = base.container.Inject<TLOG_AIPlayerKillTrackerEngineTracker_Tencent>(new TLOG_AIPlayerKillTrackerEngineTracker_Tencent());
		_nodeEnginesRoot.AddEngine(tLOG_AIPlayerKillTrackerEngineTracker_Tencent);
		CampaignWaveSummaryAnalyticsEngine campaignWaveSummaryAnalyticsEngine = new CampaignWaveSummaryAnalyticsEngine(base.container.Build<WorldSwitching>(), base.container.Build<ILobbyRequestFactory>(), base.container.Build<IServiceRequestFactory>(), base.container.Build<IAnalyticsRequestFactory>(), base.container.Build<BattlePlayers>(), base.container.Build<QuitListenerManager>(), base.container.Build<GameStateClient>(), base.container.Build<LoadingIconPresenter>());
		_nodeEnginesRoot.AddEngine(campaignWaveSummaryAnalyticsEngine);
		_contextNotifier.AddFrameworkDestructionListener(campaignWaveSummaryAnalyticsEngine);
		CampaignWaveLostLivesCountEngine campaignWaveLostLivesCountEngine = new CampaignWaveLostLivesCountEngine(base.container.Build<DestructionReporter>());
		_nodeEnginesRoot.AddEngine(campaignWaveLostLivesCountEngine);
		UpdatePlayerLivesWidgetsEngine updatePlayerLivesWidgetsEngine = base.container.Inject<UpdatePlayerLivesWidgetsEngine>(new UpdatePlayerLivesWidgetsEngine());
		_nodeEnginesRoot.AddEngine(updatePlayerLivesWidgetsEngine);
		UpdateCurrentWaveWidgetsEngine updateCurrentWaveWidgetsEngine = base.container.Inject<UpdateCurrentWaveWidgetsEngine>(new UpdateCurrentWaveWidgetsEngine());
		_nodeEnginesRoot.AddEngine(updateCurrentWaveWidgetsEngine);
		CampaignDamageBoostEngine campaignDamageBoostEngine = new CampaignDamageBoostEngine();
		_nodeEnginesRoot.AddEngine(campaignDamageBoostEngine);
		CampaignHealthBoostEngine campaignHealthBoostEngine = new CampaignHealthBoostEngine(base.container.Build<HealthTracker>());
		_nodeEnginesRoot.AddEngine(campaignHealthBoostEngine);
		TrackCampaignMatchEngine trackCampaignMatchEngine = base.container.Inject<TrackCampaignMatchEngine>(new TrackCampaignMatchEngine(base.container.Build<ISinglePlayerRequestFactory>(), base.container.Build<WeaponFireStateSync>(), _currentWaveObservable, base.container.Build<IEntityFactory>(), base.container.Build<IEntityFunctions>(), base.container.Build<DestructionManager>()));
		_nodeEnginesRoot.AddEngine(trackCampaignMatchEngine);
		_nodeEnginesRoot.AddEngine(new CampaignUpdateRemainingLivesEngine(base.container.Build<DestructionReporter>()));
		_nodeEnginesRoot.AddEngine(new CampaignDefeatCheckEngine());
		_nodeEnginesRoot.AddEngine(new CampaignVictoryCheckEngine());
		_nodeEnginesRoot.AddEngine(new CampaignTransitionAnimationEngine());
		_nodeEnginesRoot.AddEngine(new CampaignWaveUpdateKillCountEngine(base.container.Build<DestructionReporter>()));
		_nodeEnginesRoot.AddEngine(new CampaignWaveVictoryCheckEngine());
		_nodeEnginesRoot.AddEngine(new CampaignWaveUpdateTimeEngine());
		_nodeEnginesRoot.AddEngine(new CampaignWaveDefeatCheckEngine());
		_nodeEnginesRoot.AddEngine(new CampaignWaveSpawnSchedulingEngine());
		_nodeEnginesRoot.AddEngine(new CampaignWaveEnemySpawnEngine(base.container.Build<MachinePreloader>(), base.container.Build<PlayerNamesContainer>(), base.container.Build<ISpawnPointManager>(), base.container.Build<ICommandFactory>(), base.container.Build<ICubeList>(), base.container.Build<INetworkEventManagerServer>()));
		_nodeEnginesRoot.AddEngine(new CampaignWaveEnemyDespawnEngine(base.container.Build<DestructionManager>(), base.container.Build<ICommandFactory>()));
		_nodeEnginesRoot.AddEngine(new WaveGoalsWidgetShowEngine());
		RespawnAtPositionClientCommand respawnAtPositionClientCommand = base.container.Build<ICommandFactory>().Build<RespawnAtPositionClientCommand>();
		_nodeEnginesRoot.AddEngine(new CampaignWaveEnemyRespawnEngine(base.container.Build<DestructionReporter>(), respawnAtPositionClientCommand, base.container.Build<ISpawnPointManager>()));
		HealthBarShowEngine healthBarShowEngine = new HealthBarShowEngine(base.container.Build<MachineCpuDataManager>());
		_nodeEnginesRoot.AddEngine(healthBarShowEngine);
		_contextNotifier.AddFrameworkDestructionListener(healthBarShowEngine);
		CampaignEntitiesFactory.BuildEntities(base.container.Build<ISinglePlayerRequestFactory>(), base.container.Build<IEntityFactory>());
	}

	public override void SetUpEntitySystemForContextLegacy()
	{
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
		IMapModeComponent component3 = base.container.Build<IMapModeComponent>();
		base.enginesRoot.AddComponent(component3);
		_gameStartAudioTrigger = base.container.Inject<GameStartAudioTrigger>(new GameStartAudioTrigger());
		_contextNotifier.AddFrameworkInitializationListener(_gameStartAudioTrigger);
		_contextNotifier.AddFrameworkDestructionListener(_gameStartAudioTrigger);
		SinglePlayerSetupContextLegacy();
	}

	public override void BuildGameObjects()
	{
		GameObject go = _gameObjectFactory.Build("HUD_Campaigns");
		SinglePlayerCampaignScreenFactory.BuildSimulationEntities(base.container.Build<IEntityFactory>(), go, base.container.Build<PlayerMachinesContainer>());
	}

	public override void BuildClassesForContext()
	{
		base.container.Build<RoboPointAwardManager>();
		base.container.Build<TeamDeathMatchAIScoreServerMock>();
		base.container.Build<BonusManager>();
		base.container.Build<TeamBaseAudioGui>();
		base.container.Build<IInitialSimulationGUIFlow>();
		base.container.Build<SendDisconnectToServerOnWorldSwitch>();
		_gameStartAndEndTrackerTencent = base.container.Inject<GameStartAndEndTracker>(new GameStartAndEndTracker());
		_networkInitialisation.Initialize();
	}

	private void SinglePlayerSpecificBindings()
	{
		base.container.Bind<MechLegController>().AsSingle<AIMechLegController>();
		base.container.Bind<LegController>().AsSingle<AILegController>();
		base.container.BindSelf<AIPreloadRobotBuilder>();
		base.container.Bind<SinglePlayerRespawner>().AsSingle<SinglePlayerTeamDeathMatchRespawner>();
		base.container.Bind<BattleParameters>().AsSingle<BattleParametersSinglePlayer>();
		base.container.Bind<SinglePlayerWeaponFireValidator>().AsSingle<TDMPracticeWeaponFireValidator>();
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
