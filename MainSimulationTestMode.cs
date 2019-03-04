using InputMask;
using Mothership.GUI.Social;
using PlayMaker;
using RCNetwork.Client.UNet;
using RCNetwork.Events;
using RCNetwork.Server;
using Robocraft.GUI;
using Simulation;
using Simulation.Hardware.Modules;
using Simulation.SinglePlayer;
using Svelto.ServiceLayer;
using UnityEngine;
using WebServices;

internal class MainSimulationTestMode : MainSimulation
{
	public override void SetUpContainerForContext()
	{
		base.container.BindSelf<MultiplayerAvatars>();
		base.container.Bind<IPauseMenuController>().AsSingle<PauseMenuControllerTestMode>();
		base.container.Bind<ISpectatorModeActivator>().AsSingle<SinglePlayerSpectatorModeActivator>();
		base.container.Bind<IServiceRequestFactory>().AsSingle<WebStorageRequestFactoryDefault>();
		base.container.Bind<IBattleEventStreamManager>().AsSingle<BattleEventStreamManager>();
		base.container.BindSelf<RobotNameWriterPresenter>();
		base.container.Bind<IInitialSimulationGUIFlow>().AsInstance<InitialTestModeGUIFlow>(new InitialTestModeGUIFlow(new NetworkInitialisationMockClientUnity(base.container, isTestMode: true)));
		base.container.Bind<IPlayMakerStateMachineBridge>().AsSingle<PlayMakerStateMachineBridge>();
		base.container.Bind<IInputActionMask>().AsSingle<InputActionMaskTutorial>();
		base.container.BindSelf<RoboPointAwardManager>();
		base.container.Bind<SinglePlayerRespawner>().AsSingle<SinglePlayerTestModeAndTutorialRespawner>();
		base.container.BindSelf<SpectatorHintPresenter>();
		base.container.BindSelf<ReadyToRespawnPresenter>();
		base.container.BindSelf<ShowMapComponent>();
		base.container.BindSelf<SpawnMapPingComponent>();
		base.container.BindSelf<MapPingCooldownObserver>();
		base.container.BindSelf<MapPingClientCommandObserver>();
		base.container.BindSelf<MapPingCreationObserver>();
		base.container.BindSelf<PingIndicatorCreationObserver>();
		base.container.Bind<ChatPresenter>().AsSingle<ChatPresenterSinglePlayer>();
		base.container.BindSelf<ChatChannelCommands>();
		base.container.Bind<IComponentFactory>().AsSingle<GenericComponentFactory>();
		base.container.BindSelf<RespawnHealEffects>();
		base.container.BindSelf<RespawnHealthSettingsObserver>();
		base.container.BindSelf<RespawnHealEffects>();
		base.container.BindSelf<AIPreloadRobotBuilder>();
		base.container.BindSelf<SinglePlayerWeaponFireValidator>();
		base.container.Bind<IMinimapPresenter>().AsSingle<MinimapPresenter>();
	}

	public override void SetupTickablesForContext()
	{
		TestModeMaintenanceModePoller tickable = base.container.Inject<TestModeMaintenanceModePoller>(new TestModeMaintenanceModePoller());
		_tickEngine.Add(tickable);
	}

	public override void SetupNetworkContainerForContext()
	{
		base.container.Bind<INetworkEventManagerClient>().AsSingle<NetworkEventManagerClientMock>();
		base.container.Bind<INetworkEventManagerServer>().AsSingle<NetworkEventManagerServerMock>();
		base.container.Bind<ISpawnPointManager>().AsSingle<TestModeSpawnPointManager>();
		base.container.Bind<IServerTimeClient>().AsSingle<ServerTimeClientMock>();
	}

	public override void SetUpEntitySystemForContext()
	{
		NetworkStunMachineObserver observer = new NetworkStunMachineObserver(_networkStunMachineObservable);
		EmpModuleStunEngine empModuleStunEngine = base.container.Inject<EmpModuleStunEngine>(new EmpModuleStunEngine(isSinglePlayer: true, observer));
		_nodeEnginesRoot.AddEngine(empModuleStunEngine);
		_contextNotifier.AddFrameworkDestructionListener(empModuleStunEngine);
	}

	public override void SetUpEntitySystemForContextLegacy()
	{
		_guiInputController.AddDisplayScreens(new IGUIDisplay[5]
		{
			base.container.Build<IPauseMenuController>() as IGUIDisplay,
			base.container.Build<ITutorialController>() as IGUIDisplay,
			base.container.Build<ControlsDisplay>(),
			base.container.Build<SettingsDisplay>(),
			base.container.Build<GenericInfoDisplay>()
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
		IMapModeComponent component = base.container.Build<IMapModeComponent>();
		base.enginesRoot.AddComponent(component);
		ShowMapComponent component2 = base.container.Build<ShowMapComponent>();
		base.enginesRoot.AddComponent(component2);
		IPingObjectsManagementComponent component3 = base.container.Build<IPingObjectsManagementComponent>();
		base.enginesRoot.AddComponent(component3);
		SinglePlayerWeaponFireValidator component4 = base.container.Build<SinglePlayerWeaponFireValidator>();
		base.enginesRoot.AddComponent(component4);
	}

	public override void BuildGameObjects()
	{
		GameObject gameObject = _gameObjectFactory.Build("SocialPanel_Multiplayer").get_transform().GetChild(0)
			.get_gameObject();
		SocialGUIFactory socialGUIFactory = base.container.Inject<SocialGUIFactory>(new SocialGUIFactory());
		socialGUIFactory.Build(gameObject, base.container);
	}

	public override void BuildClassesForContext()
	{
		base.container.Build<IInitialSimulationGUIFlow>();
	}
}
