using Simulation;
using Simulation.Achievements;
using Simulation.BattleArena;
using Simulation.BattleArena.CapturePoint;
using Simulation.BattleArena.EnemyFusionShield;
using Simulation.BattleArena.Equalizer;
using Simulation.BattleArena.GUI;
using Simulation.SinglePlayer.CapturePoints;
using Simulation.TeamBuff;

internal class MainSimulationNormalMode : MainSimulationMultiplayer
{
	private EqualizerNotificationObservable _equalizerNotificationObservable;

	private EqualizerNotificationObserver _equalizerNotificationObserver;

	private CapturePointNotificationObservable _capturePointNotificationObservable;

	private CapturePointNotificationObserver _capturePointNotificationObserver;

	private CapturePointProgressObservable _capturePointProgressObservable;

	private CapturePointProgressObserver _capturePointProgressObserver;

	private MachineColliderIgnoreObservable _machineColliderIgnoreObservable;

	private BattleArenaGameStartAudio _baGameStartAudio;

	private PlayerCubesBuffManager _playerCubesBuffManager;

	private TeamBuffAudio _teamBuffAudio;

	private SurrenderCooldownPresenter _surrenderCooldownPresenter;

	protected override void SetUpContainerForGameMode()
	{
		PlayerQuitRequestCompleteObservable quitRequestCompleteObservable = GetQuitRequestCompleteObservable();
		PlayerQuitRequestCompleteObserver quitRequestCompleteObserver = new PlayerQuitRequestCompleteObserver(quitRequestCompleteObservable);
		base.container.Bind<IPauseMenuController>().AsInstance<PauseMenuControllerNormalMode>(new PauseMenuControllerNormalMode(GetBattleLeftObserverable(), quitRequestCompleteObserver));
		base.container.Bind<ISpectatorModeActivator>().AsSingle<SpectatorModeActivator>();
		base.container.BindSelf<ReadyToRespawnPresenter>();
		base.container.BindSelf<SpectatorHintPresenter>();
		base.container.BindSelf<TeamBaseDestructionAudioTrigger>();
		base.container.BindSelf<SupernovaPlayer>();
		base.container.BindSelf<FusionShieldActivationAudioPlayer>();
		base.container.BindSelf<TeamBasePreloader>();
		base.container.BindSelf<SurrenderCooldownPresenter>();
		base.container.BindSelf<SurrenderVotePresenter>();
		base.container.BindSelf<SurrenderControllerClient>();
		base.container.Bind<IBattleEventStreamManager>().AsSingle<BattleEventStreamManager>();
		base.container.BindSelf<BattleStatsPresenter>();
		base.container.Bind<IInitialSimulationGUIFlow>().AsSingle<InitialMultiplayerGUIFlowNormal>();
		base.container.Bind<IMinimapPresenter>().AsSingle<MinimapBattleArenaPresenter>();
		base.container.BindSelf<ShowMapComponent>();
		base.container.BindSelf<SpawnMapPingComponent>();
		base.container.BindSelf<MapPingCooldownObserver>();
		base.container.BindSelf<MapPingClientCommandObserver>();
		base.container.BindSelf<MapPingCreationObserver>();
		base.container.BindSelf<PingIndicatorCreationObserver>();
		base.container.Bind<ChatPresenter>().AsSingle<ChatPresenterMultiplayer>();
		base.container.BindSelf<TeamBaseLowHealthAudio>();
		_equalizerNotificationObservable = new EqualizerNotificationObservable();
		_equalizerNotificationObserver = new EqualizerNotificationObserver(_equalizerNotificationObservable);
		base.container.Bind<EqualizerNotificationObservable>().AsInstance<EqualizerNotificationObservable>(_equalizerNotificationObservable);
		_capturePointNotificationObservable = new CapturePointNotificationObservable();
		_capturePointNotificationObserver = new CapturePointNotificationObserver(_capturePointNotificationObservable);
		base.container.Bind<CapturePointNotificationObservable>().AsInstance<CapturePointNotificationObservable>(_capturePointNotificationObservable);
		_capturePointProgressObservable = new CapturePointProgressObservable();
		_capturePointProgressObserver = new CapturePointProgressObserver(_capturePointProgressObservable);
		base.container.Bind<CapturePointProgressObservable>().AsInstance<CapturePointProgressObservable>(_capturePointProgressObservable);
		_machineColliderIgnoreObservable = new MachineColliderIgnoreObservable();
		base.container.Bind<MachineColliderIgnoreObservable>().AsInstance<MachineColliderIgnoreObservable>(_machineColliderIgnoreObservable);
		base.container.Bind<MachineColliderIgnoreObserver>().AsInstance<MachineColliderIgnoreObserver>(new MachineColliderIgnoreObserver(_machineColliderIgnoreObservable));
		PlayerCapureStatePresenter playerCapureStatePresenter = new PlayerCapureStatePresenter(_capturePointProgressObserver);
		base.container.Bind<PlayerCapureStatePresenter>().AsInstance<PlayerCapureStatePresenter>(playerCapureStatePresenter);
		_contextNotifier.AddFrameworkDestructionListener(playerCapureStatePresenter);
		BattleArenaHUDPresenter battleArenaHUDPresenter = new BattleArenaHUDPresenter(_equalizerNotificationObserver, _capturePointNotificationObserver, _capturePointProgressObserver);
		base.container.Bind<BattleArenaHUDPresenter>().AsInstance<BattleArenaHUDPresenter>(battleArenaHUDPresenter);
		_contextNotifier.AddFrameworkDestructionListener(battleArenaHUDPresenter);
		BattleArenaMusicManager battleArenaMusicManager = new BattleArenaMusicManager(_equalizerNotificationObserver, _capturePointNotificationObserver);
		base.container.Bind<IMusicManager>().AsInstance<BattleArenaMusicManager>(battleArenaMusicManager);
	}

	protected override void SetupNetworkContainerForGameMode()
	{
		base.container.BindSelf<BonusManager>();
	}

	protected override void SetUpEntitySystemForGameMode()
	{
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
		SurrenderVotePresenter surrenderVotePresenter = base.container.Build<SurrenderVotePresenter>();
		base.enginesRoot.AddComponent(surrenderVotePresenter);
		_tickEngine.Add(surrenderVotePresenter);
		AlliesFusionShieldEngine alliesFusionShieldEngine = base.container.Inject<AlliesFusionShieldEngine>(new AlliesFusionShieldEngine());
		_contextNotifier.AddFrameworkInitializationListener(alliesFusionShieldEngine);
		_contextNotifier.AddFrameworkDestructionListener(alliesFusionShieldEngine);
		_nodeEnginesRoot.AddEngine(alliesFusionShieldEngine);
		_nodeEnginesRoot.AddEngine(base.container.Inject<FusionShieldActivator>(new FusionShieldActivator()));
		EnemyShieldDegenerateHealthEngine enemyShieldDegenerateHealthEngine = base.container.Inject<EnemyShieldDegenerateHealthEngine>(new EnemyShieldDegenerateHealthEngine());
		_nodeEnginesRoot.AddEngine(enemyShieldDegenerateHealthEngine);
		InsideFusionShieldEngine insideFusionShieldEngine = base.container.Inject<InsideFusionShieldEngine>(new InsideFusionShieldEngine());
		_nodeEnginesRoot.AddEngine(insideFusionShieldEngine);
		_contextNotifier.AddFrameworkInitializationListener(insideFusionShieldEngine);
		_contextNotifier.AddFrameworkDestructionListener(insideFusionShieldEngine);
		MachineColliderCollectionEngine machineColliderCollectionEngine = base.container.Inject<MachineColliderCollectionEngine>(new MachineColliderCollectionEngine());
		_nodeEnginesRoot.AddEngine(machineColliderCollectionEngine);
		_contextNotifier.AddFrameworkDestructionListener(machineColliderCollectionEngine);
		EnemyShieldMachineBlockerEngine enemyShieldMachineBlockerEngine = base.container.Inject<EnemyShieldMachineBlockerEngine>(new EnemyShieldMachineBlockerEngine());
		_nodeEnginesRoot.AddEngine(enemyShieldMachineBlockerEngine);
		_contextNotifier.AddFrameworkDestructionListener(enemyShieldMachineBlockerEngine);
		_nodeEnginesRoot.AddEngine(base.container.Inject<EqualizerActivationEngine>(new EqualizerActivationEngine(_equalizerNotificationObserver)));
		_nodeEnginesRoot.AddEngine(base.container.Inject<EqualizerEffectsEngine>(new EqualizerEffectsEngine(_equalizerNotificationObserver)));
		_nodeEnginesRoot.AddEngine(base.container.Inject<CapturePointAudioEngine>(new CapturePointAudioEngine(_capturePointNotificationObserver)));
		_nodeEnginesRoot.AddEngine(base.container.Inject<CapturePointEffectsEngine>(new CapturePointEffectsEngine(_capturePointNotificationObserver, _capturePointProgressObserver)));
		_nodeEnginesRoot.AddEngine(base.container.Inject<PlayerCaptureStateEngine>(new PlayerCaptureStateEngine(_capturePointNotificationObserver)));
		if (!WorldSwitching.IsCustomGame())
		{
			_nodeEnginesRoot.AddEngine(base.container.Inject<AchievementCapturePointsTrackerEngine>(new AchievementCapturePointsTrackerEngine(_capturePointNotificationObserver)));
		}
		EqualizerVOManager equalizerVOManager = base.container.Inject<EqualizerVOManager>(new EqualizerVOManager(_equalizerNotificationObserver));
		_contextNotifier.AddFrameworkDestructionListener(equalizerVOManager);
		base.container.Inject<DisconnectedPlayerVoiceOver>(new DisconnectedPlayerVoiceOver());
		ShowMapComponent component = base.container.Build<ShowMapComponent>();
		base.enginesRoot.AddComponent(component);
		IPingObjectsManagementComponent component2 = base.container.Build<IPingObjectsManagementComponent>();
		base.enginesRoot.AddComponent(component2);
		_baGameStartAudio = base.container.Inject<BattleArenaGameStartAudio>(new BattleArenaGameStartAudio());
		_playerCubesBuffManager = base.container.Inject<PlayerCubesBuffManager>(new PlayerCubesBuffManager());
		_teamBuffAudio = base.container.Inject<TeamBuffAudio>(new TeamBuffAudio());
		AICapturePointsEngine aICapturePointsEngine = base.container.Inject<AICapturePointsEngine>(new AICapturePointsEngine(_capturePointNotificationObserver));
		_nodeEnginesRoot.AddEngine(aICapturePointsEngine);
		_contextNotifier.AddFrameworkInitializationListener(aICapturePointsEngine);
		_contextNotifier.AddFrameworkDestructionListener(aICapturePointsEngine);
	}

	protected override void BuildGameObjectsForGameMode()
	{
		_gameObjectFactory.Build("HUD_Players_Crystal");
		_gameObjectFactory.Build("HUD_BattleArena");
	}

	protected override void BuildClassesForContextGameMode()
	{
		base.container.Build<TeamBaseDestructionAudioTrigger>();
		base.container.Build<FusionShieldActivationAudioPlayer>();
		base.container.Build<TeamBasePreloader>();
		base.container.Build<TeamBaseLowHealthAudio>();
		_surrenderCooldownPresenter = base.container.Build<SurrenderCooldownPresenter>();
	}
}
