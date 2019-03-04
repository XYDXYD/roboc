using Achievements;
using Avatars;
using Battle;
using ChatServiceLayer;
using ChatServiceLayer.Photon;
using EnginesGUI;
using LobbyServiceLayer;
using LobbyServiceLayer.Photon;
using ServerStateServiceLayer;
using ServerStateServiceLayer.Photon;
using Services.Analytics;
using Services.Web.Photon;
using Simulation;
using Simulation.Achievements;
using Simulation.AI;
using Simulation.Analytics;
using Simulation.BattleArena;
using Simulation.DeathEffects;
using Simulation.Destruction;
using Simulation.GUI;
using Simulation.Hardware;
using Simulation.Hardware.Cosmetic;
using Simulation.Hardware.Cosmetic.Balloon;
using Simulation.Hardware.Cosmetic.Eye;
using Simulation.Hardware.Modules;
using Simulation.Hardware.Modules.Emp.Observers;
using Simulation.Hardware.Modules.PowerModule;
using Simulation.Hardware.Modules.Sight;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Movement.Aerofoil;
using Simulation.Hardware.Movement.Hovers;
using Simulation.Hardware.Movement.InsectLegs;
using Simulation.Hardware.Movement.MechLegs;
using Simulation.Hardware.Movement.Rotors;
using Simulation.Hardware.Movement.TankTracks;
using Simulation.Hardware.Movement.Thruster;
using Simulation.Hardware.Movement.Wheeled;
using Simulation.Hardware.Movement.Wheeled.Skis;
using Simulation.Hardware.Movement.Wheeled.Wheels;
using Simulation.Hardware.Weapons;
using Simulation.Hardware.Weapons.AeroFlak;
using Simulation.Hardware.Weapons.Chaingun;
using Simulation.Hardware.Weapons.IonDistorter;
using Simulation.Hardware.Weapons.Laser;
using Simulation.Hardware.Weapons.Nano;
using Simulation.Hardware.Weapons.Plasma;
using Simulation.Hardware.Weapons.RailGun;
using Simulation.Hardware.Weapons.RocketLauncher;
using Simulation.Hardware.Weapons.Tesla;
using Simulation.Sight;
using Simulation.SinglePlayer;
using Simulation.SinglePlayer.AlignmentRectifier;
using Simulation.SinglePlayer.PowerConsumption;
using Simulation.SinglePlayer.Shooting;
using Simulation.SinglePlayer.Visibility;
using Simulation.SinglePlayer.Weapons;
using Simulation.SpawnEffects;
using Simulation.TeamBuff;
using SinglePlayerServiceLayer.Photon;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.Command;
using Svelto.Context;
using Svelto.Context.Legacy;
using Svelto.ECS;
using Svelto.ECS.Schedulers.Unity;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.IoC.Extensions.Context;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using System;
using Taunts;
using UnityEngine;

internal abstract class MainSimulation : ICompositionRoot, IUnityContextHierarchyChangedListener, IEntitySystemContext
{
	protected IContextNotifer _contextNotifier = new ContextNotifier();

	protected IGameObjectFactory _gameObjectFactory;

	internal IGUIInputControllerSim _guiInputController;

	protected NetworkStunMachineObservable _networkStunMachineObservable;

	protected EnginesRoot _nodeEnginesRoot;

	protected ITicker _tickEngine;

	protected AllowMovementObservable _allowMovementObservable;

	private AccountSanctions _accountSanctions;

	private LocalAlignmentRectifierActivatedObservable _localAlignmentRectifierActivatedObservable;

	private MachineStunnedObservable _machineStunnedObservable;

	private MaintenanceModeController _maintenanceModeController;

	private ModuleSelectedObservable _moduleSelectedObservable;

	private NetworkHitEffectObservable _networkFireObservable;

	private NetworkPlayerBlinkedObservable _networkPlayerBlinkedObservable;

	private NetworkWeaponFiredObservable _networkWeaponFiredObservable;

	private PowerUpdateObservable _powerUpdateObservable;

	private RemoteSpawnEmpLocatorObservable _remoteSpawnEmpLocatorObservable;

	private SpotStateObservable _spotStateObservable;

	private HealingAppliedObservable _healingAppliedObservable;

	private RemoteTauntObservable _remoteTauntObservable;

	private ChatCommandsSimulation _chatCommandsSimulation;

	private MachineColliderCollectionObservable _machineColliderCollectionObservable;

	private StandardChatCommands _standardChatCommands;

	private Observable<Kill> _deathAnimationFinishedObservable;

	public IContainer container
	{
		get;
		private set;
	}

	protected IEnginesRoot enginesRoot
	{
		get;
		private set;
	}

	protected MainSimulation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		ExecutionTime.Log((Action)delegate
		{
			ClearUpServiceLayer();
			GC.Collect();
			GC.WaitForPendingFinalizers();
			Resources.UnloadUnusedAssets();
			Build();
		});
	}

	protected virtual void Build()
	{
		SetupContainer();
		SetupTickables();
		SetupEntitySystemLegacy();
		SetupEntitySystem();
		SetupNewEntitySystem();
		BuildClasses();
		InitialiseStrings();
	}

	private void SetupNewEntitySystem()
	{
		LayoutAdjustmentToScreenConfigEngine layoutAdjustmentToScreenConfigEngine = container.Inject<LayoutAdjustmentToScreenConfigEngine>(new LayoutAdjustmentToScreenConfigEngine());
		RetargetableSpriteAdjustToScreenConfigEngine retargetableSpriteAdjustToScreenConfigEngine = container.Inject<RetargetableSpriteAdjustToScreenConfigEngine>(new RetargetableSpriteAdjustToScreenConfigEngine());
		RetargetableParticleAdjustToScreenConfigEngine retargetableParticleAdjustToScreenConfigEngine = container.Inject<RetargetableParticleAdjustToScreenConfigEngine>(new RetargetableParticleAdjustToScreenConfigEngine());
		_nodeEnginesRoot.AddEngine(layoutAdjustmentToScreenConfigEngine);
		_nodeEnginesRoot.AddEngine(retargetableSpriteAdjustToScreenConfigEngine);
		_nodeEnginesRoot.AddEngine(retargetableParticleAdjustToScreenConfigEngine);
	}

	void ICompositionRoot.OnContextCreated(UnityContext contextHolder)
	{
		ExecutionTime.Log((Action)delegate
		{
			Transform transform = contextHolder.get_transform();
			MonoBehaviour[] componentsInChildren = transform.GetComponentsInChildren<MonoBehaviour>(true);
			foreach (MonoBehaviour val in componentsInChildren)
			{
				if (val != null)
				{
					((IUnityContextHierarchyChangedListener)this).OnMonobehaviourAdded(val);
				}
			}
			IEntityDescriptorHolder[] componentsInChildren2 = contextHolder.GetComponentsInChildren<IEntityDescriptorHolder>();
			IEntityFactory val2 = _nodeEnginesRoot.GenerateEntityFactory();
			bool flag = false;
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				flag = false;
				IAutomaticallyBuiltEntity automaticallyBuiltEntity = componentsInChildren2[j] as IAutomaticallyBuiltEntity;
				if (automaticallyBuiltEntity != null && automaticallyBuiltEntity.InstanceCreated)
				{
					flag = true;
				}
				if (!flag)
				{
					val2.BuildEntity((componentsInChildren2[j] as MonoBehaviour).get_gameObject().GetInstanceID(), componentsInChildren2[j].RetrieveDescriptor(), (object[])(componentsInChildren2[j] as MonoBehaviour).get_gameObject().GetComponents<MonoBehaviour>());
				}
				if (componentsInChildren2[j] is IAutomaticallyBuiltEntity)
				{
					(componentsInChildren2[j] as IAutomaticallyBuiltEntity).InstanceCreated = true;
				}
			}
		});
	}

	void ICompositionRoot.OnContextInitialized()
	{
		ErrorWindow.Init(_gameObjectFactory);
		VotingAfterBattleFactory votingAfterBattleFactory = container.Build<VotingAfterBattleFactory>();
		votingAfterBattleFactory.Build(_nodeEnginesRoot);
		ExecutionTime.Log((Action)delegate
		{
			BuildGameObjects();
			_contextNotifier.NotifyFrameworkInitialized();
		});
	}

	void ICompositionRoot.OnContextDestroyed()
	{
		_contextNotifier.NotifyFrameworkDeinitialized();
		ErrorWindow.TearDown();
	}

	void IEntitySystemContext.AddComponent(IComponent component)
	{
		container.Inject<IComponent>(component);
		enginesRoot.AddComponent(component);
	}

	void IEntitySystemContext.RemoveComponent(IComponent component)
	{
		enginesRoot.RemoveComponent(component);
	}

	void IUnityContextHierarchyChangedListener.OnMonobehaviourAdded(MonoBehaviour component)
	{
		if (component is IComponent)
		{
			((IEntitySystemContext)this).AddComponent(component as IComponent);
		}
		else
		{
			container.Inject<MonoBehaviour>(component);
		}
	}

	void IUnityContextHierarchyChangedListener.OnMonobehaviourRemoved(MonoBehaviour component)
	{
		if (component is IComponent)
		{
			((IEntitySystemContext)this).RemoveComponent(component as IComponent);
		}
	}

	public abstract void SetUpContainerForContext();

	public abstract void SetupNetworkContainerForContext();

	public abstract void SetupTickablesForContext();

	public abstract void SetUpEntitySystemForContextLegacy();

	public abstract void SetUpEntitySystemForContext();

	public abstract void BuildClassesForContext();

	public abstract void BuildGameObjects();

	private void InitialiseStrings()
	{
	}

	private void SetupContainer()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		container = new ContextContainer(_contextNotifier);
		_gameObjectFactory = new GameObjectFactory(this);
		_tickEngine = new UnityTicker();
		container.Bind<IContextNotifer>().AsInstance<IContextNotifer>(_contextNotifier);
		SetupContainerCommon();
		SetUpContainerForContext();
		SetupPlatformSpecificContainer();
		SetupContainerNetwork();
		SetupNetworkContainerForContext();
	}

	private void SetupContainerCommon()
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Expected O, but got Unknown
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Expected O, but got Unknown
		container.Bind<IMonoBehaviourFactory>().AsInstance<MonoBehaviourFactory>(new MonoBehaviourFactory(this));
		container.Bind<IGameObjectFactory>().AsInstance<IGameObjectFactory>(_gameObjectFactory);
		container.BindSelf<LocalisationWrapper>();
		container.BindSelf<SocialSettings>();
		container.BindSelf<LocalisationSettings>();
		container.BindSelf<ControlsDisplay>();
		container.BindSelf<SettingsDisplay>();
		container.BindSelf<ControlsChangedObserver>();
		container.BindSelf<AdvancedRobotEditSettings>();
		container.Bind<IEntitySystemContext>().AsInstance<MainSimulation>(this);
		container.Bind<ITicker>().AsInstance<ITicker>(_tickEngine);
		container.Bind<ICubeList>().AsSingle<CubeList>();
		container.Bind<ICubeFactory>().AsSingle<SimulationCubeFactory>();
		container.Bind<ICursorMode>().AsSingle<CursorMode>();
		container.Bind<ICommandFactory>().AsInstance<CommandFactory>(new CommandFactory(container));
		container.Bind<IDispatchWorldSwitching>().AsSingle<WorldSwitching>();
		container.Bind<IPauseMenuController>().AsSingle<PauseMenuControllerTestMode>();
		TutorialTipObservable tutorialTipObservable = new TutorialTipObservable();
		container.Bind<ITutorialController>().AsInstance<SimulationTutorialController>(new SimulationTutorialController(tutorialTipObservable));
		container.Bind<PopupMessagePresenter>().AsInstance<PopupMessagePresenter>(new PopupMessagePresenter(null, new TutorialTipObserver(tutorialTipObservable)));
		container.Bind<ITeleportCooldownController>().AsSingle<TeleportCooldownController>();
		container.Bind<IMachineTeleportAudio>().AsSingle<MachineTeleportAudioFabric>();
		container.Bind<IPauseManager>().AsSingle<PauseManagerSimulation>();
		container.Bind<ILobbyRequestFactory>().AsSingle<LobbyRequestFactory>();
		SocialEventListenerFactory socialEventListenerFactory = new SocialEventListenerFactory();
		SocialEventRegistry socialEventRegistry = new SocialEventRegistry(socialEventListenerFactory);
		container.Bind<ISocialEventContainerFactory>().AsInstance<SocialEventContainerFactory>(new SocialEventContainerFactory(socialEventRegistry));
		container.Bind<ISocialRequestFactory>().AsSingle<SocialRequestFactory>();
		container.BindSelf<DisableChatInputObservable>();
		container.Bind<DisableChatInputObserver>().AsInstance<DisableChatInputObserver>(new DisableChatInputObserver(container.Build<DisableChatInputObservable>()));
		ChatEventListenerFactory chatEventListenerFactory = new ChatEventListenerFactory();
		ChatEventRegistry chatEventRegistry = new ChatEventRegistry(chatEventListenerFactory);
		container.Bind<IChatEventContainerFactory>().AsInstance<ChatEventContainerFactory>(new ChatEventContainerFactory(chatEventRegistry));
		container.Bind<IChatRequestFactory>().AsSingle<ChatRequestFactory>();
		container.Bind<IMapModeComponent>().AsSingle<MapModeComponent>();
		container.Bind<IPingObjectsManagementComponent>().AsSingle<PingObjectsManagementComponent>();
		container.Bind<IBattleStatsModeComponent>().AsSingle<BattleStatsModeComponent>();
		container.Bind<IBattleStatsInputComponent>().AsSingle<BattleStatsInputComponent>();
		ServerStateEventListenerFactory serverStateEventListenerFactory = new ServerStateEventListenerFactory();
		ServerStateEventRegistry serverStateEventRegistry = new ServerStateEventRegistry(serverStateEventListenerFactory);
		container.Bind<IServerStateEventContainerFactory>().AsInstance<ServerStateEventContainerFactory>(new ServerStateEventContainerFactory(serverStateEventRegistry));
		container.Bind<IServerStateRequestFactory>().AsSingle<ServerStateRequestFactory>();
		container.Bind<IAnalyticsRequestFactory>().AsSingle<AnalyticsRequestFactory>();
		_healingAppliedObservable = new HealingAppliedObservable();
		container.Bind<HealingAppliedObservable>().AsInstance<HealingAppliedObservable>(_healingAppliedObservable);
		_powerUpdateObservable = new PowerUpdateObservable();
		container.Bind<PowerUpdateObservable>().AsInstance<PowerUpdateObservable>(_powerUpdateObservable);
		_spotStateObservable = new SpotStateObservable();
		container.Bind<SpotStateObserver>().AsInstance<SpotStateObserver>(new SpotStateObserver(_spotStateObservable));
		_networkFireObservable = new NetworkHitEffectObservable();
		container.Bind<NetworkHitEffectObservable>().AsInstance<NetworkHitEffectObservable>(_networkFireObservable);
		_networkStunMachineObservable = new NetworkStunMachineObservable();
		container.Bind<NetworkStunMachineObservable>().AsInstance<NetworkStunMachineObservable>(_networkStunMachineObservable);
		_machineStunnedObservable = new MachineStunnedObservable();
		container.Bind<MachineStunnedObservable>().AsInstance<MachineStunnedObservable>(_machineStunnedObservable);
		container.Bind<MachineStunnedObserver>().AsInstance<MachineStunnedObserver>(new MachineStunnedObserver(_machineStunnedObservable));
		_remoteTauntObservable = new RemoteTauntObservable();
		container.Bind<RemoteTauntObservable>().AsInstance<RemoteTauntObservable>(_remoteTauntObservable);
		_remoteSpawnEmpLocatorObservable = new RemoteSpawnEmpLocatorObservable();
		container.Bind<RemoteSpawnEmpLocatorObservable>().AsInstance<RemoteSpawnEmpLocatorObservable>(_remoteSpawnEmpLocatorObservable);
		_networkWeaponFiredObservable = new NetworkWeaponFiredObservable();
		container.Bind<NetworkWeaponFiredObservable>().AsInstance<NetworkWeaponFiredObservable>(_networkWeaponFiredObservable);
		_networkPlayerBlinkedObservable = new NetworkPlayerBlinkedObservable();
		container.Bind<NetworkPlayerBlinkedObservable>().AsInstance<NetworkPlayerBlinkedObservable>(_networkPlayerBlinkedObservable);
		BuffTeamObservable buffTeamObservable = new BuffTeamObservable();
		container.Bind<BuffTeamObservable>().AsInstance<BuffTeamObservable>(buffTeamObservable);
		container.Bind<BuffTeamObserver>().AsInstance<BuffTeamObserver>(new BuffTeamObserver(buffTeamObservable));
		PlayerCubesBuffedObservable playerCubesBuffedObservable = new PlayerCubesBuffedObservable();
		container.Bind<PlayerCubesBuffedObservable>().AsInstance<PlayerCubesBuffedObservable>(playerCubesBuffedObservable);
		container.Bind<PlayerCubesBuffedObserver>().AsInstance<PlayerCubesBuffedObserver>(new PlayerCubesBuffedObserver(playerCubesBuffedObservable));
		_machineColliderCollectionObservable = new MachineColliderCollectionObservable();
		container.Bind<MachineColliderCollectionObservable>().AsInstance<MachineColliderCollectionObservable>(_machineColliderCollectionObservable);
		container.Bind<MachineColliderCollectionObserver>().AsInstance<MachineColliderCollectionObserver>(new MachineColliderCollectionObserver(_machineColliderCollectionObservable));
		_nodeEnginesRoot = new EnginesRoot(new UnitySumbmissionEntityViewScheduler());
		container.Bind<IEntityFactory>().AsInstance<IEntityFactory>(_nodeEnginesRoot.GenerateEntityFactory());
		container.Bind<IEntityFunctions>().AsInstance<IEntityFunctions>(_nodeEnginesRoot.GenerateEntityFunctions());
		container.BindSelf<MachineClusterPool>();
		container.BindSelf<MachineSphereColliderPool>();
		container.BindSelf<ChatClientProvider>();
		container.BindSelf<HUDWaitTimePresenter>();
		container.BindSelf<CubeList>();
		container.BindSelf<PlayerMachineBuiltListener>();
		container.BindSelf<MachineRootUpdater>();
		container.BindSelf<DestructionManager>();
		container.BindSelf<CubeDamagePropagator>();
		container.BindSelf<DestructionReporter>();
		container.BindSelf<CubeHealingPropagator>();
		container.BindSelf<HealingManager>();
		container.BindSelf<HealingReporter>();
		container.BindSelf<PhysicsActivatorRunner>();
		container.BindSelf<VisualDestructionEffects>();
		container.BindSelf<RobotHealthStatus>();
		container.BindSelf<RobotHealthStatusContainer>();
		container.BindSelf<WeaponFireStateSync>();
		container.BindSelf<PremiumMembership>();
		container.BindSelf<MachineSimulationBuilder>();
		container.BindSelf<WorldSwitching>();
		container.BindSelf<InputController>();
		container.Bind<IGUIInputControllerSim>().AsSingle<GUIInputControllerSim>();
		container.Bind<IGUIInputController>().AsSingle<GUIInputControllerSim>();
		container.BindSelf<CrosshairController>();
		container.BindSelf<DevMessagePresenter>();
		container.BindSelf<KillTracker>();
		container.BindSelf<HUDPlayerIDManager>();
		container.BindSelf<HUDRadarTagPresenter>();
		container.BindSelf<GameObjectPool>();
		container.BindSelf<VisibleOnScreenManager>();
		container.BindSelf<BlurEffectController>();
		container.BindSelf<MasterVolumeController>();
		container.BindSelf<MouseSettings>();
		container.BindSelf<CameraSettings>();
		container.BindSelf<LegacyControlSettings>();
		container.BindSelf<WeaponListUtility>();
		container.BindSelf<GenericInfoDisplay>();
		container.BindSelf<MachineSpawnDispatcher>();
		container.BindSelf<RemoteEnemySpottedObservable>();
		container.BindSelf<RemoteRadarActivationObservable>();
		container.BindSelf<LocalPlayerHealthBarPresenter>();
		container.BindSelf<CeilingHeightManager>();
		container.BindSelf<LegController>();
		container.BindSelf<MechLegController>();
		container.BindSelf<PhysicsStatusFactory>();
		container.BindSelf<HealthUpdateManager>();
		container.BindSelf<RemoteClientWeaponFire>();
		container.BindSelf<NanoBeamAudioManager>();
		container.BindSelf<HealthTracker>();
		container.BindSelf<LockOnNotifierController>();
		container.BindSelf<WeatherManager>();
		container.BindSelf<GameStartDispatcher>();
		container.BindSelf<GroundHeight>();
		container.BindSelf<RemoteAlignmentRectifierManager>();
		container.BindSelf<FunctionalCubesManagerFactory>();
		container.BindSelf<MachineCpuDataManager>();
		container.BindSelf<FusionShieldsObserver>();
		container.BindSelf<LocalPlayerDestructionHandler>();
		container.BindSelf<VOManager>();
		container.BindSelf<GraphicsUpdater>();
		container.BindSelf<MapDataObserver>();
		container.BindSelf<QuitListenerManager>();
		container.BindSelf<RegisterPlayerObserver>();
		container.BindSelf<GameEndedObserver>();
		container.BindSelf<SupernovaAudioObserver>();
		container.BindSelf<MachineRespawner>();
		container.BindSelf<GameStateClient>();
		container.BindSelf<CapFrameRateSettings>();
		container.BindSelf<LockedOnEffectPresenter>();
		container.BindSelf<PlayerStrafeDirectionManager>();
		container.BindSelf<LoadingIconPresenter>();
		container.Bind<ITauntMaskHelper>().AsSingle<TauntMaskHelper>();
		_localAlignmentRectifierActivatedObservable = new LocalAlignmentRectifierActivatedObservable();
		container.Bind<AlignmentRectifierEngine>().AsInstance<AlignmentRectifierEngine>(new AlignmentRectifierEngine(_localAlignmentRectifierActivatedObservable));
		_moduleSelectedObservable = new ModuleSelectedObservable();
		container.Bind<ModuleSelectedObservable>().AsInstance<ModuleSelectedObservable>(_moduleSelectedObservable);
		container.BindSelf<ZoomEngine>();
		container.BindSelf<MachineFadeEffect>();
		container.BindSelf<ShieldEntityObjectPool>();
		container.BindSelf<ModuleFireFailedObserver>();
		RemotePlayerBecomeInvisibleObservable remotePlayerBecomeInvisibleObservable = new RemotePlayerBecomeInvisibleObservable();
		container.Bind<RemotePlayerBecomeInvisibleObservable>().AsInstance<RemotePlayerBecomeInvisibleObservable>(remotePlayerBecomeInvisibleObservable);
		container.Bind<RemotePlayerBecomeInvisibleObserver>().AsInstance<RemotePlayerBecomeInvisibleObserver>(new RemotePlayerBecomeInvisibleObserver(remotePlayerBecomeInvisibleObservable));
		RemotePlayerBecomeVisibleObservable remotePlayerBecomeVisibleObservable = new RemotePlayerBecomeVisibleObservable();
		container.Bind<RemotePlayerBecomeVisibleObservable>().AsInstance<RemotePlayerBecomeVisibleObservable>(remotePlayerBecomeVisibleObservable);
		container.Bind<RemotePlayerBecomeVisibleObserver>().AsInstance<RemotePlayerBecomeVisibleObserver>(new RemotePlayerBecomeVisibleObserver(remotePlayerBecomeVisibleObservable));
		container.Bind<NetworkStunMachineObserver>().AsInstance<NetworkStunMachineObserver>(new NetworkStunMachineObserver(_networkStunMachineObservable));
		container.BindSelf<CubeDestroyedAudio>();
		container.BindSelf<MachineTeamColourUtility>();
		container.BindSelf<MultiplayerGameTimerClient>();
		container.BindSelf<FullHealAudioEngine>();
		container.Bind<MonoBehaviourPool>().ToProvider<MonoBehaviourPool>(new NamedProvider<MonoBehaviourPool>());
		container.BindSelf<ParticleSystemObjectPool>();
		container.BindSelf<ParticleSystemUpdaterObjectPool>();
		container.BindSelf<DamageIndicatorArrowPool>();
		container.BindSelf<DamageVignetteIndicatorPool>();
		container.BindSelf<ProjectileFactory>();
		container.BindSelf<DiscShieldFactory>();
		container.BindSelf<EmpTargetingLocatorFactory>();
		container.BindSelf<EmpTargetingLocatorPool>();
		container.BindSelf<EmpMainBeamFactory>();
		container.BindSelf<EmpMainBeamPool>();
		container.BindSelf<CrackDecalProjectorPool>();
		container.BindSelf<CrackDecalProjectorFactory>();
		container.BindSelf<FunctionalCubesBuilder>();
		container.BindSelf<ChatSettings>();
		container.BindSelf<ChatCommands>();
		container.BindSelf<ChatChannelContainer>();
		container.BindSelf<PrivateChat>();
		container.BindSelf<ChatAudio>();
		container.BindSelf<ChatSettings>();
		container.Bind<IgnoreList>().AsSingle<IgnoreListSimulation>();
		container.BindSelf<SwitchWeaponObserver>();
		container.BindSelf<WeaponOrderPresenter>();
		container.BindSelf<ItemDescriptorSpriteUtility>();
		container.BindSelf<SwitchWeaponAudioManager>();
		container.BindSelf<LockOnStateObservable>();
		container.BindSelf<BattlePlayers>();
		container.BindSelf<BattleTimer>();
		container.BindSelf<ForceFlushBonusObserver>();
		container.BindSelf<PresetAvatarMapProvider>();
		container.BindSelf<AvatarAvailableObservable>();
		container.Bind<AvatarAvailableObserver>().AsInstance<AvatarAvailableObserver>(new AvatarAvailableObserver(container.Build<AvatarAvailableObservable>()));
		container.BindSelf<AccountSanctionsSimulation>();
		container.BindSelf<WorldSwitchingSimulationAnalytics>();
		container.BindSelf<SkinnedMeshCreator>();
		container.BindSelf<InGamePlayerStatsUpdatedObservable>();
		container.BindSelf<UpdateVotingAfterBattleClientCommandObservable>();
		container.BindSelf<AvatarPresenterLocalPlayerIconOnly>();
		container.BindSelf<PlayerListHudPresenter>();
		container.Bind<IHudStyleController>().AsSingle<HudStyleController>();
		container.Bind<BattleLoadProgress>().AsSingle<BattleLoadProgressSimulation>();
		container.BindSelf<VotingAfterBattleFactory>();
		container.BindSelf<RobotSanctionController>();
		container.BindSelf<MachinePreloader>();
		container.BindSelf<LocalAIsContainer>();
		_allowMovementObservable = new AllowMovementObservable();
		container.Bind<AllowMovementObservable>().AsInstance<AllowMovementObservable>(_allowMovementObservable);
		container.Bind<AllowMovementObserver>().AsInstance<AllowMovementObserver>(new AllowMovementObserver(_allowMovementObservable));
		_deathAnimationFinishedObservable = new Observable<Kill>();
		container.Bind<DeathAnimationFinishedObserver>().AsInstance<DeathAnimationFinishedObserver>(new DeathAnimationFinishedObserver(_deathAnimationFinishedObservable));
	}

	private void SetupPlatformSpecificContainer()
	{
		container.BindSelf<MultiplayerBattleLongPlayMultiplier_Tencent>();
		container.Bind<IMultiAvatarLoader>().AsSingle<MultiAvatarLoader_Tencent>();
		container.Bind<IAchievementManager>().AsSingle<TencentAchievementManager>();
		container.Bind<ProfanityFilter>().AsSingle<ProfanityFilter_Tencent>();
	}

	private void SetupContainerNetwork()
	{
		container.BindSelf<ConnectedPlayersContainer>();
		container.BindSelf<PlayerNamesContainer>();
		container.BindSelf<PlayerTeamsContainer>();
		container.BindSelf<PlayerMachinesContainer>();
		container.BindSelf<LivePlayersContainer>();
		container.BindSelf<MachineRootContainer>();
		container.BindSelf<RigidbodyDataContainer>();
		container.BindSelf<MachineClusterContainer>();
		container.BindSelf<WeaponRaycastContainer>();
		container.BindSelf<NetworkMachineManager>();
		container.BindSelf<RemoteClientHistoryClient>();
		container.BindSelf<MachineTimeManager>();
		container.BindSelf<GameTimePresenter>();
		container.BindSelf<LobbyGameStartPresenter>();
		container.BindSelf<NetworkClientPool>();
		container.BindSelf<MachineSyncClient>();
	}

	private void SetupTickables()
	{
		_tickEngine.Add(new GUIShortcutTicker_Simulation(container.Build<IGUIInputControllerSim>()));
		_tickEngine.Add(container.Build<ControlsDisplay>());
		_tickEngine.Add(container.Build<CeilingHeightManager>());
		_tickEngine.Add(container.Build<WeaponFireStateSync>());
		_tickEngine.Add(container.Build<PhysicsActivatorRunner>());
		_tickEngine.Add(container.Build<GraphicsUpdater>());
		_tickEngine.Add(container.Build<PlayerStrafeDirectionManager>());
		LegController tickable = container.Build<LegController>();
		_tickEngine.Add(tickable);
		LockedOnEffectPresenter tickable2 = container.Build<LockedOnEffectPresenter>();
		_tickEngine.Add(tickable2);
		_tickEngine.Add(container.Build<VOManager>());
		SetupTickablesForContext();
	}

	private void SetupEntitySystem()
	{
		LockOnStateObservable lockOnStateObservable = container.Build<LockOnStateObservable>();
		HardwareDestroyedObservable hardwareDestroyedObservable = new HardwareDestroyedObservable();
		HardwareEnabledObservable hardwareEnabledObservable = new HardwareEnabledObservable();
		PowerUpdateObserver observer = new PowerUpdateObserver(_powerUpdateObservable);
		NetworkWeaponFiredObserver networkWeaponFiredObserver = new NetworkWeaponFiredObserver(_networkWeaponFiredObservable);
		NetworkPlayerBlinkedObserver networkPlayerBlinkedObserver = new NetworkPlayerBlinkedObserver(_networkPlayerBlinkedObservable);
		FirePressedObservable firePressedObservable = new FirePressedObservable();
		FireHeldDownObservable fireHeldDownObservable = new FireHeldDownObservable();
		TeslaFireObservable teslaFireObservable = new TeslaFireObservable();
		TauntPressedObservable tauntPressedObservable = new TauntPressedObservable();
		WeaponReadyObservable weaponReadyObservable = new WeaponReadyObservable();
		WeaponMisfiredAllObservable misfiredAllObservable = new WeaponMisfiredAllObservable();
		WeaponCooldownEndedObservable weaponCooldownEndedObservable = new WeaponCooldownEndedObservable();
		HardwareDestroyedObserver hardwareDestroyedObserver = new HardwareDestroyedObserver(hardwareDestroyedObservable);
		HardwareEnabledObserver hardwareEnabledObserver = new HardwareEnabledObserver(hardwareEnabledObservable);
		RemotePlayerBecomeInvisibleObservable observable = container.Build<RemotePlayerBecomeInvisibleObservable>();
		RemotePlayerBecomeInvisibleObserver remotePlayerBecomeInvisibleObserver = new RemotePlayerBecomeInvisibleObserver(observable);
		RemotePlayerBecomeVisibleObservable observable2 = container.Build<RemotePlayerBecomeVisibleObservable>();
		RemotePlayerBecomeVisibleObserver remotePlayerBecomeVisibleObserver = new RemotePlayerBecomeVisibleObserver(observable2);
		WeaponFiredObservable weaponFiredObservable = new WeaponFiredObservable();
		WeaponFiredObserver weaponFiredObserver = new WeaponFiredObserver(weaponFiredObservable);
		NetworkHitEffectObserver networkFireObserver = new NetworkHitEffectObserver(_networkFireObservable);
		WeaponNoFireObservable weaponNoFireObservable = new WeaponNoFireObservable();
		TeamRadarObservable teamRadarObservable = new TeamRadarObservable();
		RobotShakeEngine robotShakeEngine = container.Inject<RobotShakeEngine>(new RobotShakeEngine());
		_nodeEnginesRoot.AddEngine(robotShakeEngine);
		CameraShakeEngine cameraShakeEngine = container.Inject<CameraShakeEngine>(new CameraShakeEngine());
		_nodeEnginesRoot.AddEngine(cameraShakeEngine);
		DamageVignetteEngine damageVignetteEngine = container.Inject<DamageVignetteEngine>(new DamageVignetteEngine());
		_nodeEnginesRoot.AddEngine(damageVignetteEngine);
		_nodeEnginesRoot.AddEngine(container.Inject<SkinnedMeshUpdateEngine>(new SkinnedMeshUpdateEngine()));
		LocalPlayerSwitchWeaponEngine localPlayerSwitchWeaponEngine = container.Inject<LocalPlayerSwitchWeaponEngine>(new LocalPlayerSwitchWeaponEngine(hardwareDestroyedObserver, hardwareEnabledObserver, _moduleSelectedObservable));
		_contextNotifier.AddFrameworkDestructionListener(localPlayerSwitchWeaponEngine);
		_nodeEnginesRoot.AddEngine(localPlayerSwitchWeaponEngine);
		enginesRoot.AddComponent(localPlayerSwitchWeaponEngine);
		AliveStateEngine aliveStateEngine = container.Inject<AliveStateEngine>(new AliveStateEngine());
		_contextNotifier.AddFrameworkDestructionListener(aliveStateEngine);
		_nodeEnginesRoot.AddEngine(aliveStateEngine);
		RemoteSwitchWeaponEngine remoteSwitchWeaponEngine = container.Inject<RemoteSwitchWeaponEngine>(new RemoteSwitchWeaponEngine());
		_nodeEnginesRoot.AddEngine(remoteSwitchWeaponEngine);
		HardwareHealthStatusEngine hardwareHealthStatusEngine = container.Inject<HardwareHealthStatusEngine>(new HardwareHealthStatusEngine(hardwareDestroyedObservable, hardwareEnabledObservable));
		_nodeEnginesRoot.AddEngine(hardwareHealthStatusEngine);
		PowerModuleEffectsEngine powerModuleEffectsEngine = container.Inject<PowerModuleEffectsEngine>(new PowerModuleEffectsEngine(observer));
		_nodeEnginesRoot.AddEngine(powerModuleEffectsEngine);
		PowerModuleEngine powerModuleEngine = container.Inject<PowerModuleEngine>(new PowerModuleEngine());
		_nodeEnginesRoot.AddEngine(powerModuleEngine);
		enginesRoot.AddComponent(new EmpModuleInputDisablingComponent(new MachineStunnedObserver(_machineStunnedObservable)));
		RemoteSpawnEmpLocatorObserver observer2 = new RemoteSpawnEmpLocatorObserver(_remoteSpawnEmpLocatorObservable);
		EmpModuleLocatorSpawnerEngine empModuleLocatorSpawnerEngine = container.Inject<EmpModuleLocatorSpawnerEngine>(new EmpModuleLocatorSpawnerEngine(observer2));
		_nodeEnginesRoot.AddEngine(empModuleLocatorSpawnerEngine);
		_contextNotifier.AddFrameworkDestructionListener(empModuleLocatorSpawnerEngine);
		_contextNotifier.AddFrameworkInitializationListener(empModuleLocatorSpawnerEngine);
		EmpLocatorCountdownManagementEngine empLocatorCountdownManagementEngine = container.Inject<EmpLocatorCountdownManagementEngine>(new EmpLocatorCountdownManagementEngine());
		_nodeEnginesRoot.AddEngine(empLocatorCountdownManagementEngine);
		_tickEngine.Add(empLocatorCountdownManagementEngine);
		EmpLocatorRaycastDownEngine empLocatorRaycastDownEngine = container.Inject<EmpLocatorRaycastDownEngine>(new EmpLocatorRaycastDownEngine());
		_nodeEnginesRoot.AddEngine(empLocatorRaycastDownEngine);
		MachineStunManagementEngine machineStunManagementEngine = container.Inject<MachineStunManagementEngine>(new MachineStunManagementEngine());
		_nodeEnginesRoot.AddEngine(machineStunManagementEngine);
		_tickEngine.Add(machineStunManagementEngine);
		LoadEmpModuleStatsEngine loadEmpModuleStatsEngine = container.Inject<LoadEmpModuleStatsEngine>(new LoadEmpModuleStatsEngine());
		_nodeEnginesRoot.AddEngine(loadEmpModuleStatsEngine);
		NetworkStunMachineObserver observer3 = new NetworkStunMachineObserver(_networkStunMachineObservable);
		EmpModuleEffectsEngine empModuleEffectsEngine = container.Inject<EmpModuleEffectsEngine>(new EmpModuleEffectsEngine(observer3));
		_nodeEnginesRoot.AddEngine(empModuleEffectsEngine);
		_contextNotifier.AddFrameworkDestructionListener(empModuleEffectsEngine);
		PowerBarEngine powerBarEngine = new PowerBarEngine(weaponFiredObserver, container.Build<IServiceRequestFactory>(), container.Build<MachineSpawnDispatcher>());
		_nodeEnginesRoot.AddEngine(powerBarEngine);
		_contextNotifier.AddFrameworkDestructionListener(powerBarEngine);
		MachineInputEngine machineInputEngine = container.Inject<MachineInputEngine>(new MachineInputEngine(firePressedObservable, fireHeldDownObservable, new LocalAlignmentRectifierActivatedObserver(_localAlignmentRectifierActivatedObservable), tauntPressedObservable));
		_nodeEnginesRoot.AddEngine(machineInputEngine);
		_tickEngine.Add(machineInputEngine);
		RemoteRadarActivationObserver remoteRadarActivationObserver = new RemoteRadarActivationObserver(container.Build<RemoteRadarActivationObservable>());
		RadarEngine radarEngine = container.Inject<RadarEngine>(new RadarEngine(container.Build<INetworkEventManagerClient>(), remoteRadarActivationObserver, teamRadarObservable));
		_nodeEnginesRoot.AddEngine(radarEngine);
		_nodeEnginesRoot.AddEngine(container.Inject<RadarFeedbackEngine>(new RadarFeedbackEngine()));
		TauntEngine tauntEngine = container.Inject<TauntEngine>(new TauntEngine(new TauntPressedObserver(tauntPressedObservable), new RemoteTauntObserver(_remoteTauntObservable)));
		_nodeEnginesRoot.AddEngine(tauntEngine);
		_contextNotifier.AddFrameworkInitializationListener(tauntEngine);
		_contextNotifier.AddFrameworkDestructionListener(tauntEngine);
		TeamRadarObserver teamRadarObserver = new TeamRadarObserver(teamRadarObservable);
		RadarTagEngine radarTagEngine = new RadarTagEngine(teamRadarObserver);
		_nodeEnginesRoot.AddEngine(container.Inject<RadarTagEngine>(radarTagEngine));
		RemoteEnemySpottedObserver remoteEnemySpottedObserver = new RemoteEnemySpottedObserver(container.Build<RemoteEnemySpottedObservable>());
		AutoSpotEngine autoSpotEngine = container.Inject<AutoSpotEngine>(new AutoSpotEngine(container.Build<INetworkEventManagerClient>(), remoteEnemySpottedObserver, _spotStateObservable, teamRadarObserver));
		_nodeEnginesRoot.AddEngine(autoSpotEngine);
		LoadRadarModuleStatsEngine loadRadarModuleStatsEngine = container.Inject<LoadRadarModuleStatsEngine>(new LoadRadarModuleStatsEngine());
		_nodeEnginesRoot.AddEngine(loadRadarModuleStatsEngine);
		_nodeEnginesRoot.AddEngine(container.Inject<LoadWeaponStatsEngine>(new LoadWeaponStatsEngine()));
		_nodeEnginesRoot.AddEngine(container.Inject<LoadChaingunWeaponStatsEngine>(new LoadChaingunWeaponStatsEngine()));
		_nodeEnginesRoot.AddEngine(container.Inject<LoadPlasmaWeaponStatsEngine>(new LoadPlasmaWeaponStatsEngine()));
		_nodeEnginesRoot.AddEngine(container.Inject<LoadTeslaWeaponStatsEngine>(new LoadTeslaWeaponStatsEngine()));
		_nodeEnginesRoot.AddEngine(container.Inject<LoadAeroflakWeaponStatsEngine>(new LoadAeroflakWeaponStatsEngine()));
		_nodeEnginesRoot.AddEngine(container.Inject<LoadLockOnWeaponStatsEngine>(new LoadLockOnWeaponStatsEngine()));
		NextWeaponToFireEngine nextWeaponToFireEngine = container.Inject<NextWeaponToFireEngine>(new NextWeaponToFireEngine(new WeaponReadyObserver(weaponReadyObservable), networkWeaponFiredObserver));
		_nodeEnginesRoot.AddEngine(nextWeaponToFireEngine);
		_contextNotifier.AddFrameworkDestructionListener(nextWeaponToFireEngine);
		SmartWeaponFireEngine smartWeaponFireEngine = container.Inject<SmartWeaponFireEngine>(new SmartWeaponFireEngine(misfiredAllObservable));
		_nodeEnginesRoot.AddEngine(smartWeaponFireEngine);
		_tickEngine.Add(smartWeaponFireEngine);
		TeslaWeaponFireEngine teslaWeaponFireEngine = container.Inject<TeslaWeaponFireEngine>(new TeslaWeaponFireEngine());
		_nodeEnginesRoot.AddEngine(teslaWeaponFireEngine);
		RailAimGraphicsEngine railAimGraphicsEngine = container.Inject<RailAimGraphicsEngine>(new RailAimGraphicsEngine());
		_nodeEnginesRoot.AddEngine(railAimGraphicsEngine);
		_tickEngine.Add(railAimGraphicsEngine);
		WeaponAimEngine weaponAimEngine = container.Inject<WeaponAimEngine>(new WeaponAimEngine());
		_nodeEnginesRoot.AddEngine(weaponAimEngine);
		_tickEngine.Add(weaponAimEngine);
		WeaponFireTimingEngine weaponFireTimingEngine = container.Inject<WeaponFireTimingEngine>(new WeaponFireTimingEngine(weaponReadyObservable, weaponNoFireObservable, new FirePressedObserver(firePressedObservable), new FireHeldDownObserver(fireHeldDownObservable), new TeslaFireObserver(teslaFireObservable), weaponCooldownEndedObservable));
		_nodeEnginesRoot.AddEngine(weaponFireTimingEngine);
		_tickEngine.Add(weaponFireTimingEngine);
		_contextNotifier.AddFrameworkDestructionListener(weaponFireTimingEngine);
		ChaingunSpinEngine chaingunSpinEngine = container.Inject<ChaingunSpinEngine>(new ChaingunSpinEngine(networkWeaponFiredObserver));
		_nodeEnginesRoot.AddEngine(chaingunSpinEngine);
		ChaingunSpinEffectEngine chaingunSpinEffectEngine = container.Inject<ChaingunSpinEffectEngine>(new ChaingunSpinEffectEngine());
		_nodeEnginesRoot.AddEngine(chaingunSpinEffectEngine);
		_tickEngine.Add(chaingunSpinEffectEngine);
		ChaingunSpinAudioEngine chaingunSpinAudioEngine = container.Inject<ChaingunSpinAudioEngine>(new ChaingunSpinAudioEngine());
		_nodeEnginesRoot.AddEngine(chaingunSpinAudioEngine);
		_tickEngine.Add(chaingunSpinAudioEngine);
		ShellParticlesEngine shellParticlesEngine = container.Inject<ShellParticlesEngine>(new ShellParticlesEngine());
		_nodeEnginesRoot.AddEngine(shellParticlesEngine);
		WeaponAccuracyEngine weaponAccuracyEngine = container.Inject<WeaponAccuracyEngine>(new WeaponAccuracyEngine());
		_nodeEnginesRoot.AddEngine(weaponAccuracyEngine);
		_tickEngine.Add(weaponAccuracyEngine);
		BasicShootingEngine basicShootingEngine = container.Inject<BasicShootingEngine>(new BasicShootingEngine(new WeaponNoFireObserver(weaponNoFireObservable), networkWeaponFiredObserver));
		_nodeEnginesRoot.AddEngine(basicShootingEngine);
		_contextNotifier.AddFrameworkDestructionListener(basicShootingEngine);
		LaserWeaponShootingEngine laserWeaponShootingEngine = container.Inject<LaserWeaponShootingEngine>(new LaserWeaponShootingEngine(weaponFiredObservable));
		_nodeEnginesRoot.AddEngine(laserWeaponShootingEngine);
		PlasmaWeaponShootingEngine plasmaWeaponShootingEngine = container.Inject<PlasmaWeaponShootingEngine>(new PlasmaWeaponShootingEngine(weaponFiredObservable));
		_nodeEnginesRoot.AddEngine(plasmaWeaponShootingEngine);
		AeroflakWeaponShootingEngine aeroflakWeaponShootingEngine = container.Inject<AeroflakWeaponShootingEngine>(new AeroflakWeaponShootingEngine(weaponFiredObservable));
		_nodeEnginesRoot.AddEngine(aeroflakWeaponShootingEngine);
		RocketLauncherShootingEngine rocketLauncherShootingEngine = container.Inject<RocketLauncherShootingEngine>(new RocketLauncherShootingEngine(weaponFiredObservable, new LockOnStateObserver(lockOnStateObservable)));
		_nodeEnginesRoot.AddEngine(rocketLauncherShootingEngine);
		_tickEngine.Add(rocketLauncherShootingEngine);
		IonDistorterShootingEngine ionDistorterShootingEngine = container.Inject<IonDistorterShootingEngine>(new IonDistorterShootingEngine(weaponFiredObservable));
		_nodeEnginesRoot.AddEngine(ionDistorterShootingEngine);
		_nodeEnginesRoot.AddEngine(container.Inject<RecycleProjectileOnResetEngine>(new RecycleProjectileOnResetEngine()));
		ShootingAfterEffectsEngine shootingAfterEffectsEngine = container.Inject<ShootingAfterEffectsEngine>(new ShootingAfterEffectsEngine(container.Build<IServiceRequestFactory>()));
		_nodeEnginesRoot.AddEngine(shootingAfterEffectsEngine);
		ShootAnimationEngine shootAnimationEngine = container.Inject<ShootAnimationEngine>(new ShootAnimationEngine());
		_nodeEnginesRoot.AddEngine(shootAnimationEngine);
		GenericProjectileTrailEngine genericProjectileTrailEngine = container.Inject<GenericProjectileTrailEngine>(new GenericProjectileTrailEngine());
		_nodeEnginesRoot.AddEngine(genericProjectileTrailEngine);
		_tickEngine.Add(genericProjectileTrailEngine);
		RailProjectileTrailEngine railProjectileTrailEngine = container.Inject<RailProjectileTrailEngine>(new RailProjectileTrailEngine());
		_nodeEnginesRoot.AddEngine(railProjectileTrailEngine);
		_tickEngine.Add(railProjectileTrailEngine);
		StraightProjectileCollisionEngine straightProjectileCollisionEngine = container.Inject<StraightProjectileCollisionEngine>(new StraightProjectileCollisionEngine());
		_nodeEnginesRoot.AddEngine(straightProjectileCollisionEngine);
		_tickEngine.Add(straightProjectileCollisionEngine);
		StraightProjectileEngine straightProjectileEngine = container.Inject<StraightProjectileEngine>(new StraightProjectileEngine());
		_nodeEnginesRoot.AddEngine(straightProjectileEngine);
		_tickEngine.Add(straightProjectileEngine);
		PlasmaProjectileEngine plasmaProjectileEngine = container.Inject<PlasmaProjectileEngine>(new PlasmaProjectileEngine());
		_nodeEnginesRoot.AddEngine(plasmaProjectileEngine);
		_tickEngine.Add(plasmaProjectileEngine);
		IonDistorterProjectileEngine ionDistorterProjectileEngine = container.Inject<IonDistorterProjectileEngine>(new IonDistorterProjectileEngine());
		_nodeEnginesRoot.AddEngine(ionDistorterProjectileEngine);
		_tickEngine.Add(ionDistorterProjectileEngine);
		IonDistorterCollisionEngine ionDistorterCollisionEngine = container.Inject<IonDistorterCollisionEngine>(new IonDistorterCollisionEngine());
		_nodeEnginesRoot.AddEngine(ionDistorterCollisionEngine);
		StackDamageBonusEngine stackDamageBonusEngine = container.Inject<StackDamageBonusEngine>(new StackDamageBonusEngine());
		_nodeEnginesRoot.AddEngine(stackDamageBonusEngine);
		AeroflakProjectileEngine aeroflakProjectileEngine = container.Inject<AeroflakProjectileEngine>(new AeroflakProjectileEngine());
		_nodeEnginesRoot.AddEngine(aeroflakProjectileEngine);
		_tickEngine.Add(aeroflakProjectileEngine);
		LaserDamageEngine laserDamageEngine = container.Inject<LaserDamageEngine>(new LaserDamageEngine());
		_nodeEnginesRoot.AddEngine(laserDamageEngine);
		RailDamageEngine railDamageEngine = container.Inject<RailDamageEngine>(new RailDamageEngine());
		_nodeEnginesRoot.AddEngine(railDamageEngine);
		HealingProjectileImpactEngine healingProjectileImpactEngine = container.Inject<HealingProjectileImpactEngine>(new HealingProjectileImpactEngine());
		_nodeEnginesRoot.AddEngine(healingProjectileImpactEngine);
		IonDistorterDamageEngine ionDistorterDamageEngine = container.Inject<IonDistorterDamageEngine>(new IonDistorterDamageEngine());
		_nodeEnginesRoot.AddEngine(ionDistorterDamageEngine);
		LaserWeaponEffectsEngine laserWeaponEffectsEngine = container.Inject<LaserWeaponEffectsEngine>(new LaserWeaponEffectsEngine(networkFireObserver));
		_nodeEnginesRoot.AddEngine(laserWeaponEffectsEngine);
		PlasmaWeaponEffectsEngine plasmaWeaponEffectsEngine = container.Inject<PlasmaWeaponEffectsEngine>(new PlasmaWeaponEffectsEngine(networkFireObserver));
		_nodeEnginesRoot.AddEngine(plasmaWeaponEffectsEngine);
		AeroflakWeaponEffectsEngine aeroflakWeaponEffectsEngine = container.Inject<AeroflakWeaponEffectsEngine>(new AeroflakWeaponEffectsEngine(networkFireObserver));
		_nodeEnginesRoot.AddEngine(aeroflakWeaponEffectsEngine);
		IonDistorterEffectsEngine ionDistorterEffectsEngine = container.Inject<IonDistorterEffectsEngine>(new IonDistorterEffectsEngine(networkFireObserver));
		_nodeEnginesRoot.AddEngine(ionDistorterEffectsEngine);
		ZoomEngine zoomEngine = container.Build<ZoomEngine>();
		_nodeEnginesRoot.AddEngine(zoomEngine);
		CrosshairWeaponTrackerEngine crosshairWeaponTrackerEngine = container.Inject<CrosshairWeaponTrackerEngine>(new CrosshairWeaponTrackerEngine());
		_nodeEnginesRoot.AddEngine(crosshairWeaponTrackerEngine);
		AimAngleCrosshairUpdaterEngine aimAngleCrosshairUpdaterEngine = container.Inject<AimAngleCrosshairUpdaterEngine>(new AimAngleCrosshairUpdaterEngine());
		_nodeEnginesRoot.AddEngine(aimAngleCrosshairUpdaterEngine);
		_nodeEnginesRoot.AddEngine(container.Inject<HealingProjectileImpactEffectEngine>(new HealingProjectileImpactEffectEngine(networkFireObserver)));
		_nodeEnginesRoot.AddEngine(container.Inject<RocketLauncherEffectsEngine>(new RocketLauncherEffectsEngine(networkFireObserver)));
		HomingProjectileEngine homingProjectileEngine = container.Inject<HomingProjectileEngine>(new HomingProjectileEngine(networkPlayerBlinkedObserver, remotePlayerBecomeInvisibleObserver));
		_nodeEnginesRoot.AddEngine(homingProjectileEngine);
		_tickEngine.Add(homingProjectileEngine);
		RocketLauncherProjectileTrailEngine rocketLauncherProjectileTrailEngine = container.Inject<RocketLauncherProjectileTrailEngine>(new RocketLauncherProjectileTrailEngine());
		_nodeEnginesRoot.AddEngine(rocketLauncherProjectileTrailEngine);
		_tickEngine.Add(rocketLauncherProjectileTrailEngine);
		RocketLauncherImpactEngine rocketLauncherImpactEngine = container.Inject<RocketLauncherImpactEngine>(new RocketLauncherImpactEngine());
		_nodeEnginesRoot.AddEngine(rocketLauncherImpactEngine);
		LockOnTargeterEngine lockOnTargeterEngine = container.Inject<LockOnTargeterEngine>(new LockOnTargeterEngine(lockOnStateObservable, hardwareDestroyedObserver, hardwareEnabledObserver, remotePlayerBecomeInvisibleObserver, weaponFiredObserver));
		_nodeEnginesRoot.AddEngine(lockOnTargeterEngine);
		_tickEngine.Add(lockOnTargeterEngine);
		TeslaRamEngine teslaRamEngine = container.Inject<TeslaRamEngine>(new TeslaRamEngine(weaponFiredObservable, teslaFireObservable, networkWeaponFiredObserver));
		_nodeEnginesRoot.AddEngine(teslaRamEngine);
		_tickEngine.Add(teslaRamEngine);
		_contextNotifier.AddFrameworkDestructionListener(teslaRamEngine);
		TeslaCollisionEngine teslaCollisionEngine = container.Inject<TeslaCollisionEngine>(new TeslaCollisionEngine());
		_nodeEnginesRoot.AddEngine(teslaCollisionEngine);
		_nodeEnginesRoot.AddEngine(container.Inject<TeslaRamEffectsEngine>(new TeslaRamEffectsEngine(networkFireObserver)));
		NanoCrosshairUpdaterEngine nanoCrosshairUpdaterEngine = container.Inject<NanoCrosshairUpdaterEngine>(new NanoCrosshairUpdaterEngine());
		_nodeEnginesRoot.AddEngine(nanoCrosshairUpdaterEngine);
		_tickEngine.Add(nanoCrosshairUpdaterEngine);
		ManaDrainingEngine manaDrainingEngine = container.Inject<ManaDrainingEngine>(new ManaDrainingEngine());
		_nodeEnginesRoot.AddEngine(manaDrainingEngine);
		_tickEngine.Add(manaDrainingEngine);
		MachineGroundedEngine machineGroundedEngine = container.Inject<MachineGroundedEngine>(new MachineGroundedEngine());
		_nodeEnginesRoot.AddEngine(machineGroundedEngine);
		ModuleActivationEngine moduleActivationEngine = container.Inject<ModuleActivationEngine>(new ModuleActivationEngine(new ModuleSelectedObserver(_moduleSelectedObservable)));
		_nodeEnginesRoot.AddEngine(moduleActivationEngine);
		_contextNotifier.AddFrameworkDestructionListener(moduleActivationEngine);
		ModuleStartCooldownEngine moduleStartCooldownEngine = container.Inject<ModuleStartCooldownEngine>(new ModuleStartCooldownEngine());
		_nodeEnginesRoot.AddEngine(moduleStartCooldownEngine);
		ModuleReadyEffectEngine moduleReadyEffectEngine = container.Inject<ModuleReadyEffectEngine>(new ModuleReadyEffectEngine());
		_nodeEnginesRoot.AddEngine(moduleReadyEffectEngine);
		DiscShieldSpawnerEngine discShieldSpawnerEngine = container.Inject<DiscShieldSpawnerEngine>(new DiscShieldSpawnerEngine());
		_nodeEnginesRoot.AddEngine(discShieldSpawnerEngine);
		_contextNotifier.AddFrameworkInitializationListener(discShieldSpawnerEngine);
		DiscShieldManagerEngine discShieldManagerEngine = container.Inject<DiscShieldManagerEngine>(new DiscShieldManagerEngine());
		_nodeEnginesRoot.AddEngine(discShieldManagerEngine);
		_tickEngine.Add(discShieldManagerEngine);
		DiscShieldEffectsEngine discShieldEffectsEngine = container.Inject<DiscShieldEffectsEngine>(new DiscShieldEffectsEngine());
		_nodeEnginesRoot.AddEngine(discShieldEffectsEngine);
		_tickEngine.Add(discShieldEffectsEngine);
		DiscShieldAudioEngine discShieldAudioEngine = container.Inject<DiscShieldAudioEngine>(new DiscShieldAudioEngine());
		_nodeEnginesRoot.AddEngine(discShieldAudioEngine);
		LoadDiscShieldStatsEngine loadDiscShieldStatsEngine = container.Inject<LoadDiscShieldStatsEngine>(new LoadDiscShieldStatsEngine());
		_nodeEnginesRoot.AddEngine(loadDiscShieldStatsEngine);
		ModuleActivationGUIEngine moduleActivationGUIEngine = container.Inject<ModuleActivationGUIEngine>(new ModuleActivationGUIEngine(hardwareDestroyedObserver, hardwareEnabledObserver));
		_nodeEnginesRoot.AddEngine(moduleActivationGUIEngine);
		_tickEngine.Add(moduleActivationGUIEngine);
		_contextNotifier.AddFrameworkDestructionListener(moduleActivationGUIEngine);
		MachineInvisibilityEngine machineInvisibilityEngine = container.Inject<MachineInvisibilityEngine>(new MachineInvisibilityEngine(remotePlayerBecomeInvisibleObserver, remotePlayerBecomeVisibleObserver, weaponFiredObserver));
		_nodeEnginesRoot.AddEngine(machineInvisibilityEngine);
		_contextNotifier.AddFrameworkDestructionListener(machineInvisibilityEngine);
		LoadModuleStatsEngine loadModuleStatsEngine = container.Inject<LoadModuleStatsEngine>(new LoadModuleStatsEngine());
		_nodeEnginesRoot.AddEngine(loadModuleStatsEngine);
		LoadInvisibilityStatsEngine loadInvisibilityStatsEngine = container.Inject<LoadInvisibilityStatsEngine>(new LoadInvisibilityStatsEngine());
		_nodeEnginesRoot.AddEngine(loadInvisibilityStatsEngine);
		TeleportModuleActivationEngine teleportModuleActivationEngine = container.Inject<TeleportModuleActivationEngine>(new TeleportModuleActivationEngine());
		_nodeEnginesRoot.AddEngine(teleportModuleActivationEngine);
		_contextNotifier.AddFrameworkInitializationListener(teleportModuleActivationEngine);
		TeleportModuleTeleporterEngine teleportModuleTeleporterEngine = container.Inject<TeleportModuleTeleporterEngine>(new TeleportModuleTeleporterEngine());
		_nodeEnginesRoot.AddEngine(teleportModuleTeleporterEngine);
		_tickEngine.Add(teleportModuleTeleporterEngine);
		_contextNotifier.AddFrameworkDestructionListener(teleportModuleTeleporterEngine);
		TeleportModuleEffectsEngine teleportModuleEffectsEngine = container.Inject<TeleportModuleEffectsEngine>(new TeleportModuleEffectsEngine(networkPlayerBlinkedObserver));
		_nodeEnginesRoot.AddEngine(teleportModuleEffectsEngine);
		TeleportModuleCameraEffectsEngine teleportModuleCameraEffectsEngine = container.Inject<TeleportModuleCameraEffectsEngine>(new TeleportModuleCameraEffectsEngine());
		_nodeEnginesRoot.AddEngine(teleportModuleCameraEffectsEngine);
		_tickEngine.Add(teleportModuleCameraEffectsEngine);
		LoadTeleportModuleStatsEngine loadTeleportModuleStatsEngine = container.Inject<LoadTeleportModuleStatsEngine>(new LoadTeleportModuleStatsEngine());
		_nodeEnginesRoot.AddEngine(loadTeleportModuleStatsEngine);
		CrosshairWeaponNoFireStateTrackerEngine crosshairWeaponNoFireStateTrackerEngine = container.Inject<CrosshairWeaponNoFireStateTrackerEngine>(new CrosshairWeaponNoFireStateTrackerEngine(new WeaponReadyObserver(weaponReadyObservable), hardwareDestroyedObserver, hardwareEnabledObserver, new WeaponCooldownEndedObserver(weaponCooldownEndedObservable)));
		_nodeEnginesRoot.AddEngine(crosshairWeaponNoFireStateTrackerEngine);
		_contextNotifier.AddFrameworkDestructionListener(crosshairWeaponNoFireStateTrackerEngine);
		LoadMovementStatsEngine loadMovementStatsEngine = container.Inject<LoadMovementStatsEngine>(new LoadMovementStatsEngine());
		_nodeEnginesRoot.AddEngine(loadMovementStatsEngine);
		TopSpeedEngine topSpeedEngine = container.Inject<TopSpeedEngine>(new TopSpeedEngine());
		_nodeEnginesRoot.AddEngine(topSpeedEngine);
		PlayerThrustersManagerEngine playerThrustersManagerEngine = container.Inject<PlayerThrustersManagerEngine>(new PlayerThrustersManagerEngine());
		_nodeEnginesRoot.AddEngine(playerThrustersManagerEngine);
		_tickEngine.Add(playerThrustersManagerEngine);
		ThrusterEngine thrusterEngine = container.Inject<ThrusterEngine>(new ThrusterEngine());
		_nodeEnginesRoot.AddEngine(thrusterEngine);
		MachineThrusterAudioEngine machineThrusterAudioEngine = container.Inject<MachineThrusterAudioEngine>(new MachineThrusterAudioEngine());
		_nodeEnginesRoot.AddEngine(machineThrusterAudioEngine);
		_tickEngine.Add(machineThrusterAudioEngine);
		MachinePropellersAudioEngine machinePropellersAudioEngine = container.Inject<MachinePropellersAudioEngine>(new MachinePropellersAudioEngine());
		_nodeEnginesRoot.AddEngine(machinePropellersAudioEngine);
		PropellerEffectsEngine propellerEffectsEngine = container.Inject<PropellerEffectsEngine>(new PropellerEffectsEngine());
		_nodeEnginesRoot.AddEngine(propellerEffectsEngine);
		_tickEngine.Add(propellerEffectsEngine);
		LocalRotorBladeEngine localRotorBladeEngine = container.Inject<LocalRotorBladeEngine>(new LocalRotorBladeEngine());
		_nodeEnginesRoot.AddEngine(localRotorBladeEngine);
		_tickEngine.Add(localRotorBladeEngine);
		ReomteRotorBladeEngine reomteRotorBladeEngine = container.Inject<ReomteRotorBladeEngine>(new ReomteRotorBladeEngine());
		_nodeEnginesRoot.AddEngine(reomteRotorBladeEngine);
		_tickEngine.Add(reomteRotorBladeEngine);
		RotorGraphicsEngine rotorGraphicsEngine = container.Inject<RotorGraphicsEngine>(new RotorGraphicsEngine());
		_nodeEnginesRoot.AddEngine(rotorGraphicsEngine);
		_tickEngine.Add(rotorGraphicsEngine);
		RotorAudioEngine rotorAudioEngine = container.Inject<RotorAudioEngine>(new RotorAudioEngine());
		_nodeEnginesRoot.AddEngine(rotorAudioEngine);
		WheelColliderEngine wheelColliderEngine = container.Inject<WheelColliderEngine>(new WheelColliderEngine());
		_nodeEnginesRoot.AddEngine(wheelColliderEngine);
		HighSpeedColliderEngine highSpeedColliderEngine = container.Inject<HighSpeedColliderEngine>(new HighSpeedColliderEngine());
		_nodeEnginesRoot.AddEngine(highSpeedColliderEngine);
		_tickEngine.Add(highSpeedColliderEngine);
		CameraRelativeTurnDampingEngine cameraRelativeTurnDampingEngine = container.Inject<CameraRelativeTurnDampingEngine>(new CameraRelativeTurnDampingEngine());
		_nodeEnginesRoot.AddEngine(cameraRelativeTurnDampingEngine);
		_tickEngine.Add(cameraRelativeTurnDampingEngine);
		TankTrackEngine tankTrackEngine = container.Inject<TankTrackEngine>(new TankTrackEngine());
		_nodeEnginesRoot.AddEngine(tankTrackEngine);
		_tickEngine.Add(tankTrackEngine);
		TankTrackManagerEngine tankTrackManagerEngine = container.Inject<TankTrackManagerEngine>(new TankTrackManagerEngine());
		_nodeEnginesRoot.AddEngine(tankTrackManagerEngine);
		_tickEngine.Add(tankTrackManagerEngine);
		SingleTankTrackWheelColliderActivatorEngine singleTankTrackWheelColliderActivatorEngine = container.Inject<SingleTankTrackWheelColliderActivatorEngine>(new SingleTankTrackWheelColliderActivatorEngine());
		_nodeEnginesRoot.AddEngine(singleTankTrackWheelColliderActivatorEngine);
		TankTrackAudioEngine tankTrackAudioEngine = container.Inject<TankTrackAudioEngine>(new TankTrackAudioEngine());
		_nodeEnginesRoot.AddEngine(tankTrackAudioEngine);
		TankTrackGraphicsEngine tankTrackGraphicsEngine = new TankTrackGraphicsEngine();
		_nodeEnginesRoot.AddEngine(tankTrackGraphicsEngine);
		_tickEngine.Add(tankTrackGraphicsEngine);
		WheeledMachineAudioEngine wheeledMachineAudioEngine = container.Inject<WheeledMachineAudioEngine>(new WheeledMachineAudioEngine());
		_nodeEnginesRoot.AddEngine(wheeledMachineAudioEngine);
		WheelGraphicsEngine wheelGraphicsEngine = container.Inject<WheelGraphicsEngine>(new WheelGraphicsEngine());
		_nodeEnginesRoot.AddEngine(wheelGraphicsEngine);
		SkiMachineAudioEngine skiMachineAudioEngine = container.Inject<SkiMachineAudioEngine>(new SkiMachineAudioEngine());
		_nodeEnginesRoot.AddEngine(skiMachineAudioEngine);
		SkiGraphicsEngine skiGraphicsEngine = container.Inject<SkiGraphicsEngine>(new SkiGraphicsEngine());
		_nodeEnginesRoot.AddEngine(skiGraphicsEngine);
		WheeledMachineEngine wheeledMachineEngine = container.Inject<WheeledMachineEngine>(new WheeledMachineEngine());
		_nodeEnginesRoot.AddEngine(wheeledMachineEngine);
		MechLegMachineEngine mechLegMachineEngine = container.Inject<MechLegMachineEngine>(new MechLegMachineEngine());
		_nodeEnginesRoot.AddEngine(mechLegMachineEngine);
		InsectLegMachineEngine insectLegMachineEngine = container.Inject<InsectLegMachineEngine>(new InsectLegMachineEngine());
		_nodeEnginesRoot.AddEngine(insectLegMachineEngine);
		AerofoilEngines();
		HoverEngines();
		CameraRelativeInputEngine cameraRelativeInputEngine = container.Inject<CameraRelativeInputEngine>(new CameraRelativeInputEngine());
		_nodeEnginesRoot.AddEngine(cameraRelativeInputEngine);
		_tickEngine.Add(cameraRelativeInputEngine);
		DamageMultiplierEngine damageMultiplierEngine = container.Inject<DamageMultiplierEngine>(new DamageMultiplierEngine());
		_nodeEnginesRoot.AddEngine(damageMultiplierEngine);
		WeaponBlockerEngine weaponBlockerEngine = container.Inject<WeaponBlockerEngine>(new WeaponBlockerEngine());
		_nodeEnginesRoot.AddEngine(weaponBlockerEngine);
		DamageBoostEngine damageBoostEngine = container.Inject<DamageBoostEngine>(new DamageBoostEngine());
		_nodeEnginesRoot.AddEngine(damageBoostEngine);
		HealingPriorityEngine healingPriorityEngine = container.Inject<HealingPriorityEngine>(new HealingPriorityEngine(new HealingAppliedObserver(_healingAppliedObservable)));
		_nodeEnginesRoot.AddEngine(healingPriorityEngine);
		EyeEngine eyeEngine = container.Inject<EyeEngine>(new EyeEngine());
		_nodeEnginesRoot.AddEngine(eyeEngine);
		BalloonEngine balloonEngine = container.Inject<BalloonEngine>(new BalloonEngine());
		_nodeEnginesRoot.AddEngine(balloonEngine);
		PlaySoundWhenParticleDestroyedEngine playSoundWhenParticleDestroyedEngine = container.Inject<PlaySoundWhenParticleDestroyedEngine>(new PlaySoundWhenParticleDestroyedEngine());
		_nodeEnginesRoot.AddEngine(playSoundWhenParticleDestroyedEngine);
		MachinePhysicsActivationEngine machinePhysicsActivationEngine = container.Inject<MachinePhysicsActivationEngine>(new MachinePhysicsActivationEngine(new AllowMovementObserver(_allowMovementObservable)));
		_nodeEnginesRoot.AddEngine(machinePhysicsActivationEngine);
		_contextNotifier.AddFrameworkInitializationListener(machinePhysicsActivationEngine);
		_contextNotifier.AddFrameworkDestructionListener(machinePhysicsActivationEngine);
		KillAssistBonusEngine killAssistBonusEngine = new KillAssistBonusEngine(container.Build<DestructionReporter>(), container.Build<INetworkEventManagerClient>(), container.Build<HealthTracker>());
		_nodeEnginesRoot.AddEngine(killAssistBonusEngine);
		_contextNotifier.AddFrameworkDestructionListener(killAssistBonusEngine);
		HealAssistBonusEngine healAssistBonusEngine = new HealAssistBonusEngine(container.Build<HealthTracker>(), container.Build<INetworkEventManagerClient>());
		_nodeEnginesRoot.AddEngine(healAssistBonusEngine);
		_contextNotifier.AddFrameworkDestructionListener(healAssistBonusEngine);
		AutoHealthRegenEngine autoHealthRegenEngine = container.Inject<AutoHealthRegenEngine>(new AutoHealthRegenEngine());
		_contextNotifier.AddFrameworkDestructionListener(autoHealthRegenEngine);
		_nodeEnginesRoot.AddEngine(autoHealthRegenEngine);
		AutoHealthRegenGuiEngine autoHealthRegenGuiEngine = container.Inject<AutoHealthRegenGuiEngine>(new AutoHealthRegenGuiEngine());
		_contextNotifier.AddFrameworkDestructionListener(autoHealthRegenGuiEngine);
		_nodeEnginesRoot.AddEngine(autoHealthRegenGuiEngine);
		enginesRoot.AddEngine(autoHealthRegenGuiEngine);
		MachineTargetsEngine machineTargetsEngine = new MachineTargetsEngine(container.Build<MachinePreloader>());
		_nodeEnginesRoot.AddEngine(machineTargetsEngine);
		FrameRateTrackerEngine frameRateTrackerEngine = new FrameRateTrackerEngine(container.Build<IAnalyticsRequestFactory>());
		_nodeEnginesRoot.AddEngine(frameRateTrackerEngine);
		_contextNotifier.AddFrameworkDestructionListener(frameRateTrackerEngine);
		ExhaustEffectEngine exhaustEffectEngine = new ExhaustEffectEngine();
		_nodeEnginesRoot.AddEngine(exhaustEffectEngine);
		MachineMotorDetectionEngine machineMotorDetectionEngine = new MachineMotorDetectionEngine();
		_nodeEnginesRoot.AddEngine(machineMotorDetectionEngine);
		MachinePlaySpawnEffectEngine machinePlaySpawnEffectEngine = new MachinePlaySpawnEffectEngine(container.Build<MachineSpawnDispatcher>());
		_nodeEnginesRoot.AddEngine(machinePlaySpawnEffectEngine);
		_contextNotifier.AddFrameworkDestructionListener(machinePlaySpawnEffectEngine);
		MachinePlayDeathEffectEngine machinePlayDeathEffectEngine = new MachinePlayDeathEffectEngine(container.Build<DestructionReporter>(), _deathAnimationFinishedObservable, container.Build<PlayerMachinesContainer>(), container.Build<PlayerTeamsContainer>());
		_nodeEnginesRoot.AddEngine(machinePlayDeathEffectEngine);
		_contextNotifier.AddFrameworkDestructionListener(machinePlayDeathEffectEngine);
		PowerConsumptionSequencer powerConsumptionSequencer = new PowerConsumptionSequencer();
		PowerConsumptionSequenceEngine powerConsumptionSequenceEngine = new PowerConsumptionSequenceEngine(powerConsumptionSequencer);
		_contextNotifier.AddFrameworkInitializationListener(powerConsumptionSequenceEngine);
		_contextNotifier.AddFrameworkDestructionListener(powerConsumptionSequenceEngine);
		_nodeEnginesRoot.AddEngine(powerConsumptionSequenceEngine);
		powerConsumptionSequencer.SetSequence(powerConsumptionSequenceEngine, powerBarEngine, chaingunSpinEngine, crosshairWeaponNoFireStateTrackerEngine);
		SetUpAIEngines();
		SetUpEntitySystemForContext();
	}

	private void HoverEngines()
	{
		HoverBladeGFXEngine hoverBladeGFXEngine = container.Inject<HoverBladeGFXEngine>(new HoverBladeGFXEngine());
		_nodeEnginesRoot.AddEngine(hoverBladeGFXEngine);
		HoverBladeEngine hoverBladeEngine = container.Inject<HoverBladeEngine>(new HoverBladeEngine());
		_nodeEnginesRoot.AddEngine(hoverBladeEngine);
		HoverBladeAFXEngine hoverBladeAFXEngine = container.Inject<HoverBladeAFXEngine>(new HoverBladeAFXEngine());
		_nodeEnginesRoot.AddEngine(hoverBladeAFXEngine);
	}

	private void AerofoilEngines()
	{
		AerofoilPhysicEngine aerofoilPhysicEngine = container.Inject<AerofoilPhysicEngine>(new AerofoilPhysicEngine());
		_nodeEnginesRoot.AddEngine(aerofoilPhysicEngine);
		AerofoilUpdateEngine aerofoilUpdateEngine = container.Inject<AerofoilUpdateEngine>(new AerofoilUpdateEngine());
		_nodeEnginesRoot.AddEngine(aerofoilUpdateEngine);
		AerofoilRemoteGFXEngine aerofoilRemoteGFXEngine = container.Inject<AerofoilRemoteGFXEngine>(new AerofoilRemoteGFXEngine());
		_nodeEnginesRoot.AddEngine(aerofoilRemoteGFXEngine);
		AerofoilLocalGFXEngine aerofoilLocalGFXEngine = container.Inject<AerofoilLocalGFXEngine>(new AerofoilLocalGFXEngine());
		_nodeEnginesRoot.AddEngine(aerofoilLocalGFXEngine);
		AerofoilMachineAFXEngine aerofoilMachineAFXEngine = container.Inject<AerofoilMachineAFXEngine>(new AerofoilMachineAFXEngine());
		_nodeEnginesRoot.AddEngine(aerofoilMachineAFXEngine);
	}

	private void SetupEntitySystemLegacy()
	{
		enginesRoot = new EnginesRoot();
		ICursorMode cursorMode = container.Build<ICursorMode>();
		_guiInputController = container.Build<IGUIInputControllerSim>();
		EditingInputPlugin editingInputPlugin = container.Inject<EditingInputPlugin>(new EditingInputPlugin());
		WorldSwitchInputPlugin worldSwitchInputPlugin = container.Inject<WorldSwitchInputPlugin>(new WorldSwitchInputPlugin());
		CharacterInputPlugin characterInputPlugin = container.Inject<CharacterInputPlugin>(new CharacterInputPlugin(cursorMode));
		InputEngine inputEngine = new InputEngine(container.Build<InputController>(), characterInputPlugin, worldSwitchInputPlugin, editingInputPlugin);
		enginesRoot.AddEngine(inputEngine);
		_tickEngine.Add(inputEngine);
		GUIFadeAwayEngine gUIFadeAwayEngine = container.Inject<GUIFadeAwayEngine>(new GUIFadeAwayEngine());
		enginesRoot.AddEngine(gUIFadeAwayEngine);
		_tickEngine.Add(gUIFadeAwayEngine);
		enginesRoot.AddEngine(new WorldSwitchingEngine(container.Build<IDispatchWorldSwitching>()));
		AlignmentRectifierEngine alignmentRectifierEngine = container.Build<AlignmentRectifierEngine>();
		enginesRoot.AddEngine(alignmentRectifierEngine);
		enginesRoot.AddComponent(alignmentRectifierEngine);
		_nodeEnginesRoot.AddEngine(alignmentRectifierEngine);
		MachineRespawner tickable = container.Build<MachineRespawner>();
		_tickEngine.Add(tickable);
		SupernovaAudioEngine engine = container.Inject<SupernovaAudioEngine>(new SupernovaAudioEngine());
		enginesRoot.AddEngine(engine);
		SetUpEntitySystemForContextLegacy();
	}

	private void SetUpAIEngines()
	{
		AIEngine aIEngine = container.Inject<AIEngine>(new AIEngine(new AllowMovementObserver(_allowMovementObservable)));
		_nodeEnginesRoot.AddEngine(aIEngine);
		_contextNotifier.AddFrameworkInitializationListener(aIEngine);
		_contextNotifier.AddFrameworkDestructionListener(aIEngine);
		AIWaypointEngine aIWaypointEngine = container.Inject<AIWaypointEngine>(new AIWaypointEngine());
		_nodeEnginesRoot.AddEngine(aIWaypointEngine);
		AIWeaponAccuracyEngine aIWeaponAccuracyEngine = container.Inject<AIWeaponAccuracyEngine>(new AIWeaponAccuracyEngine());
		_nodeEnginesRoot.AddEngine(aIWeaponAccuracyEngine);
		_contextNotifier.AddFrameworkInitializationListener(aIWeaponAccuracyEngine);
		AIAlignmentRectifierEngine aIAlignmentRectifierEngine = container.Inject<AIAlignmentRectifierEngine>(new AIAlignmentRectifierEngine());
		_nodeEnginesRoot.AddEngine(aIAlignmentRectifierEngine);
		_tickEngine.Add(aIAlignmentRectifierEngine);
		AIWeaponShootingFeedbackEngine aIWeaponShootingFeedbackEngine = container.Inject<AIWeaponShootingFeedbackEngine>(new AIWeaponShootingFeedbackEngine());
		_nodeEnginesRoot.AddEngine(aIWeaponShootingFeedbackEngine);
		AIWeaponPowerConsumptionEngine aIWeaponPowerConsumptionEngine = container.Inject<AIWeaponPowerConsumptionEngine>(new AIWeaponPowerConsumptionEngine());
		_nodeEnginesRoot.AddEngine(aIWeaponPowerConsumptionEngine);
		_tickEngine.Add(aIWeaponPowerConsumptionEngine);
		AISwitchWeaponEngine aISwitchWeaponEngine = container.Inject<AISwitchWeaponEngine>(new AISwitchWeaponEngine());
		_nodeEnginesRoot.AddEngine(aISwitchWeaponEngine);
		AIBotVisibilityEngine aIBotVisibilityEngine = container.Inject<AIBotVisibilityEngine>(new AIBotVisibilityEngine());
		_nodeEnginesRoot.AddEngine(aIBotVisibilityEngine);
		_contextNotifier.AddFrameworkInitializationListener(aIBotVisibilityEngine);
	}

	private void BuildClasses()
	{
		container.Build<KillTracker>();
		container.Build<HealthUpdateManager>();
		container.Build<LocalPlayerDestructionHandler>();
		container.Build<CubeDestroyedAudio>();
		container.Build<FullHealAudioEngine>();
		CapFrameRateSettings capFrameRateSettings = container.Build<CapFrameRateSettings>();
		capFrameRateSettings.InitialiseFrameRate();
		container.Build<MultiplayerBattleLongPlayMultiplier_Tencent>();
		container.Inject<FunctionalCubesBuilder>(new FunctionalCubesBuilder());
		_standardChatCommands = container.Inject<StandardChatCommands>(new StandardChatCommands());
		_contextNotifier.AddFrameworkDestructionListener(_standardChatCommands);
		_accountSanctions = container.Build<AccountSanctionsSimulation>();
		_contextNotifier.AddFrameworkDestructionListener(_accountSanctions);
		_maintenanceModeController = container.Inject<MaintenanceModeController>(new MaintenanceModeController());
		_contextNotifier.AddFrameworkInitializationListener(_maintenanceModeController);
		_contextNotifier.AddFrameworkDestructionListener(_maintenanceModeController);
		container.Inject<PreWorldswitchSimulation>(new PreWorldswitchSimulation());
		container.Inject<SwitchWeaponAudioManager>(new SwitchWeaponAudioManager());
		_chatCommandsSimulation = container.Inject<ChatCommandsSimulation>(new ChatCommandsSimulation());
		WorldSwitchingSimulationAnalytics worldSwitchingSimulationAnalytics = container.Build<WorldSwitchingSimulationAnalytics>();
		FloatingNumbersController floatingNumbersController = new FloatingNumbersController(container.Build<HealthTracker>());
		container.Bind<FloatingNumbersController>().AsInstance<FloatingNumbersController>(floatingNumbersController);
		KillNotificationController killNotificationController = new KillNotificationController(container.Build<PlayerTeamsContainer>(), container.Build<PlayerNamesContainer>());
		container.Bind<KillNotificationController>().AsInstance<KillNotificationController>(killNotificationController);
		container.Bind<MultiplayerGameTimerClient>();
		AudioLanguageSetter audioLanguageSetter = container.Inject<AudioLanguageSetter>(new AudioLanguageSetter());
		_contextNotifier.AddFrameworkDestructionListener(audioLanguageSetter);
		GameStartedImplementor gameStartedImplementor = new GameStartedImplementor();
		container.Build<IEntityFactory>().BuildEntity<GameEntityDescriptor>(300, new object[1]
		{
			gameStartedImplementor
		});
		BuildClassesForContext();
	}

	private void ClearUpServiceLayer()
	{
		PhotonWebServicesUtility.TearDown();
		PhotonChatUtility.TearDown();
		PhotonSocialUtility.TearDown();
		PhotonSinglePlayerUtility.TearDown();
		PhotonLobbyUtility.TearDown();
	}
}
