using Simulation;
using Simulation.Pit;
using Simulation.TeamBuff;

internal class MainSimulationTeamDeathMatchMode : MainSimulationMultiplayer
{
	private TDMGameStartAudio _tdmGameStartAudio;

	private PlayerCubesBuffManager _playerCubesBuffManager;

	private TeamBuffAudio _teamBuffAudio;

	private SurrenderCooldownPresenter _surrenderCooldownPresenter;

	protected override void SetUpContainerForGameMode()
	{
		base.container.Bind<IPauseMenuController>().AsInstance<PauseMenuControllerBasicMode>(new PauseMenuControllerBasicMode(GetBattleLeftObserverable(), new PlayerQuitRequestCompleteObserver(GetQuitRequestCompleteObservable())));
		base.container.Bind<IBattleEventStreamManager>().AsSingle<BattleEventStreamManager>();
		base.container.BindSelf<BattleStatsPresenter>();
		base.container.Bind<ISpectatorModeActivator>().AsSingle<SpectatorModeActivatorPit>();
		base.container.BindSelf<SpectatorHintPresenter>();
		base.container.BindSelf<ReadyToRespawnPresenter>();
		base.container.BindSelf<SurrenderCooldownPresenter>();
		base.container.BindSelf<SurrenderVotePresenter>();
		base.container.BindSelf<SurrenderControllerClient>();
		base.container.Bind<IInitialSimulationGUIFlow>().AsSingle<InitialMultiplayerGUIFlow>();
		base.container.BindSelf<ShowMapComponent>();
		base.container.BindSelf<SpawnMapPingComponent>();
		base.container.BindSelf<MapPingCooldownObserver>();
		base.container.BindSelf<MapPingClientCommandObserver>();
		base.container.BindSelf<MapPingCreationObserver>();
		base.container.BindSelf<PingIndicatorCreationObserver>();
		base.container.Bind<ChatPresenter>().AsSingle<ChatPresenterMultiplayer>();
		base.container.Bind<IMinimapPresenter>().AsSingle<MinimapPresenter>();
		base.container.BindSelf<TeamDeathMatchStatsPresenter>();
		base.container.Bind<IMusicManager>().AsSingle<TDMMusicManager>();
		base.container.BindSelf<TDMMusicManager>();
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
		base.container.Inject<DisconnectedTeamDeathmatchPlayerVoiceOver>(new DisconnectedTeamDeathmatchPlayerVoiceOver());
		ShowMapComponent component = base.container.Build<ShowMapComponent>();
		base.enginesRoot.AddComponent(component);
		IPingObjectsManagementComponent component2 = base.container.Build<IPingObjectsManagementComponent>();
		base.enginesRoot.AddComponent(component2);
	}

	protected override void BuildGameObjectsForGameMode()
	{
		_gameObjectFactory.Build("HUD_TeamDeathmatch5v5");
		_gameObjectFactory.Build("HUD_Players");
	}

	protected override void BuildClassesForContextGameMode()
	{
		_tdmGameStartAudio = base.container.Inject<TDMGameStartAudio>(new TDMGameStartAudio());
		_playerCubesBuffManager = base.container.Inject<PlayerCubesBuffManager>(new PlayerCubesBuffManager());
		_teamBuffAudio = base.container.Inject<TeamBuffAudio>(new TeamBuffAudio());
		_surrenderCooldownPresenter = base.container.Build<SurrenderCooldownPresenter>();
	}
}
