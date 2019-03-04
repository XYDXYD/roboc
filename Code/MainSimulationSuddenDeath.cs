using Simulation;

internal class MainSimulationSuddenDeath : MainSimulationMultiplayer
{
	protected override void SetUpContainerForGameMode()
	{
		base.container.Bind<IPauseMenuController>().AsInstance<PauseMenuControllerClassicMode>(new PauseMenuControllerClassicMode(GetBattleLeftObserverable()));
		base.container.Bind<ISpectatorModeActivator>().AsSingle<ClassicModeSpectatorModeActivator>();
		base.container.BindSelf<TeamBaseStatsPresenter>();
		base.container.Bind<IBattleEventStreamManager>().AsSingle<BattleEventStreamManager>();
		base.container.BindSelf<BattleStatsPresenter>();
		base.container.Bind<IInitialSimulationGUIFlow>().AsSingle<InitialMultiplayerGUIFlow>();
		base.container.Bind<IMinimapPresenter>().AsSingle<MinimapPresenter>();
		base.container.BindSelf<ShowMapComponent>();
		base.container.BindSelf<SpawnMapPingComponent>();
		base.container.BindSelf<MapPingCooldownObserver>();
		base.container.BindSelf<MapPingClientCommandObserver>();
		base.container.BindSelf<MapPingCreationObserver>();
		base.container.BindSelf<PingIndicatorCreationObserver>();
		base.container.Bind<ChatPresenter>().AsSingle<ChatPresenterMultiplayer>();
		base.container.Bind<IMusicManager>().AsSingle<SuddenDeathMusicManager>();
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
		ShowMapComponent component = base.container.Build<ShowMapComponent>();
		base.enginesRoot.AddComponent(component);
		IPingObjectsManagementComponent component2 = base.container.Build<IPingObjectsManagementComponent>();
		base.enginesRoot.AddComponent(component2);
	}

	protected override void BuildGameObjectsForGameMode()
	{
		_gameObjectFactory.Build("HUD_Players");
		_gameObjectFactory.Build("HUD_TeamBaseStats");
		_gameObjectFactory.Build("HUD_GameTimer");
	}

	protected override void BuildClassesForContextGameMode()
	{
	}
}
