using Battle;
using InputMask;
using Mothership.GUI.Social;
using RCNetwork.Events;
using RCNetwork.UNet.Client;
using Simulation;
using Simulation.Achievements;
using Simulation.Battle;
using Simulation.BattleTracker;
using Simulation.Hardware.Modules;
using Simulation.SinglePlayer;
using Simulation.TeamBuff;
using Svelto.ECS;
using Svelto.ServiceLayer;
using UnityEngine;
using WebServices;

internal abstract class MainSimulationMultiplayer : MainSimulation
{
	private PlayerQuitRequestCompleteObservable _quitRequestCompleteObservable = new PlayerQuitRequestCompleteObservable();

	private BattleLeftEventObservable _battleLeftObservable = new BattleLeftEventObservable();

	private BattleStatsPresenter _battleStatsPresenter;

	private GameStartAudioTrigger _gameStartAudioTrigger;

	private NetworkInitialisationSimulationLiteNetLib _networkInitialisation;

	private TeamBaseUnderAttackVoiceOverEngine _teamBaseUnderAttackVoiceOver;

	private MachineMotionSenderEngine _machineMotionSenderEngine;

	private GameStartAndEndTracker _gameStartAndEndTrackerTencent;

	public sealed override void SetUpContainerForContext()
	{
		base.container.Bind<IInputActionMask>().AsSingle<InputActionMaskNormal>();
		base.container.Bind<BattleParameters>().AsSingle<BattleParametersSimulation>();
		SetUpContainerForGameMode();
		base.container.BindSelf<MultiplayerAvatars>();
		base.container.BindSelf<RoboPointAwardManager>();
		base.container.BindSelf<HealCubesBonusManager>();
		base.container.BindSelf<ProtectTeamMateBonusManager>();
		base.container.BindSelf<DefendTheBaseBonusManager>();
		base.container.BindSelf<TeamBaseProgressDispatcher>();
		base.container.BindSelf<TeamBaseAudioGui>();
		base.container.BindSelf<NetworkInputRecievedManager>();
		base.container.BindSelf<ClientPing>();
		base.container.BindSelf<DestructionSyncReplayer>();
		base.container.BindSelf<SendDisconnectToServerOnWorldSwitch>();
		base.container.Bind<ChatWarningDialogue>().AsInstance<ChatWarningDialogue>(new ChatWarningDialogue());
		base.container.BindSelf<RobotNameWriterPresenter>();
		base.container.BindSelf<RespawnHealEffects>();
		base.container.BindSelf<RespawnHealthSettingsObserver>();
		base.container.Bind<PlayerQuitRequestCompleteObservable>().AsInstance<PlayerQuitRequestCompleteObservable>(_quitRequestCompleteObservable);
		base.container.BindSelf<AIPreloadRobotBuilder>();
		base.container.BindSelf<AISpawnerMultiplayer>();
		if (WorldSwitching.IsBrawl())
		{
			base.container.Bind<IServiceRequestFactory>().AsSingle<WebStorageRequestFactoryBrawl>();
		}
		else if (WorldSwitching.IsCustomGame())
		{
			base.container.Bind<IServiceRequestFactory>().AsSingle<WebStorageRequestFactoryCustomGame>();
		}
		else
		{
			base.container.Bind<IServiceRequestFactory>().AsSingle<WebStorageRequestFactoryDefault>();
		}
	}

	public sealed override void SetupNetworkContainerForContext()
	{
		base.container.Bind<INetworkEventManagerClient>().AsSingle<NetworkEventManagerClientLiteNetLib>();
		base.container.Bind<IServerTimeClient>().AsSingle<ServerTimeClient>();
		base.container.Bind<ILobbyPlayerListPresenter>().AsSingle<LobbyPlayerListPresenter>();
		SetupNetworkContainerForGameMode();
	}

	public sealed override void SetupTickablesForContext()
	{
		_tickEngine.Add(base.container.Build<ClientPing>());
	}

	public sealed override void SetUpEntitySystemForContext()
	{
		NetworkStunMachineObserver observer = new NetworkStunMachineObserver(_networkStunMachineObservable);
		EmpModuleStunEngine empModuleStunEngine = base.container.Inject<EmpModuleStunEngine>(new EmpModuleStunEngine(isSinglePlayer: false, observer));
		_nodeEnginesRoot.AddEngine(empModuleStunEngine);
		_contextNotifier.AddFrameworkDestructionListener(empModuleStunEngine);
		WeaponDamageBuffEngine weaponDamageBuffEngine = base.container.Inject<WeaponDamageBuffEngine>(new WeaponDamageBuffEngine());
		_nodeEnginesRoot.AddEngine(weaponDamageBuffEngine);
		TLOG_PlayerKillTrackerDataCacheFactory_Tencent tLOG_PlayerKillTrackerDataCacheFactory_Tencent = new TLOG_PlayerKillTrackerDataCacheFactory_Tencent(base.container.Build<IEntityFactory>(), base.container, _nodeEnginesRoot);
		tLOG_PlayerKillTrackerDataCacheFactory_Tencent.BuildEntities();
		tLOG_PlayerKillTrackerDataCacheFactory_Tencent.BuildEngine();
		TLOG_PlayerKillTrackerEngine_Tencent tLOG_PlayerKillTrackerEngine_Tencent = base.container.Inject<TLOG_PlayerKillTrackerEngine_Tencent>(new TLOG_PlayerKillTrackerEngine_Tencent());
		_nodeEnginesRoot.AddEngine(tLOG_PlayerKillTrackerEngine_Tencent);
		_machineMotionSenderEngine = new MachineMotionSenderEngine(base.container.Build<MachineSpawnDispatcher>(), base.container.Build<GameStateClient>());
		_nodeEnginesRoot.AddEngine(_machineMotionSenderEngine);
		if (!WorldSwitching.IsCustomGame())
		{
			LocalPlayerMadeKillObservable observable = new LocalPlayerMadeKillObservable();
			LocalPlayerMadeKillObserver localPlayerMadeKillObserver = new LocalPlayerMadeKillObserver(observable);
			LocalPlayerHealedOtherToFullHealthObservable observable2 = new LocalPlayerHealedOtherToFullHealthObservable();
			LocalPlayerHealedOtherToFullHealthObserver localPlayerHealedOtherToFullHealthObserver = new LocalPlayerHealedOtherToFullHealthObserver(observable2);
			_nodeEnginesRoot.AddEngine(base.container.Inject<AchievementKillWithTeslaAfterDecloakTrackerEngine>(new AchievementKillWithTeslaAfterDecloakTrackerEngine()));
			_nodeEnginesRoot.AddEngine(base.container.Inject<AchievementModuleActivatedTrackerEngine>(new AchievementModuleActivatedTrackerEngine()));
			_nodeEnginesRoot.AddEngine(base.container.Inject<AchievementGameEndTrackerEngine>(new AchievementGameEndTrackerEngine()));
			_nodeEnginesRoot.AddEngine(base.container.Inject<LocalPlayerKillTrackerEngine>(new LocalPlayerKillTrackerEngine(observable)));
			base.container.Inject<AchievementHealerTracker>(new AchievementHealerTracker(localPlayerHealedOtherToFullHealthObserver));
			base.container.Inject<AchievementKillTracker>(new AchievementKillTracker(localPlayerMadeKillObserver));
			base.container.Inject<LocalPlayerHealedOtherToFullHealthTracker>(new LocalPlayerHealedOtherToFullHealthTracker(observable2));
			base.container.Inject<DailyQuestTracker>(new DailyQuestTracker(localPlayerMadeKillObserver, localPlayerHealedOtherToFullHealthObserver));
		}
	}

	public sealed override void SetUpEntitySystemForContextLegacy()
	{
		BattleStatsEngine engine = base.container.Inject<BattleStatsEngine>(new BattleStatsEngine());
		base.enginesRoot.AddEngine(engine);
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
		SetUpEntitySystemForGameMode();
		IBattleStatsModeComponent component = base.container.Build<IBattleStatsModeComponent>();
		base.enginesRoot.AddComponent(component);
		IBattleStatsInputComponent component2 = base.container.Build<IBattleStatsInputComponent>();
		base.enginesRoot.AddComponent(component2);
		IMapModeComponent component3 = base.container.Build<IMapModeComponent>();
		base.enginesRoot.AddComponent(component3);
		_teamBaseUnderAttackVoiceOver = base.container.Inject<TeamBaseUnderAttackVoiceOverEngine>(new TeamBaseUnderAttackVoiceOverEngine());
		_contextNotifier.AddFrameworkInitializationListener(_teamBaseUnderAttackVoiceOver);
		_contextNotifier.AddFrameworkDestructionListener(_teamBaseUnderAttackVoiceOver);
		_gameStartAudioTrigger = base.container.Inject<GameStartAudioTrigger>(new GameStartAudioTrigger());
		_contextNotifier.AddFrameworkInitializationListener(_gameStartAudioTrigger);
		_contextNotifier.AddFrameworkDestructionListener(_gameStartAudioTrigger);
		_gameStartAndEndTrackerTencent = base.container.Inject<GameStartAndEndTracker>(new GameStartAndEndTracker());
	}

	public sealed override void BuildGameObjects()
	{
		BuildGameObjectsForGameMode();
		GameObject gameObject = _gameObjectFactory.Build("SocialPanel_Multiplayer").get_transform().GetChild(0)
			.get_gameObject();
		SocialGUIFactory socialGUIFactory = base.container.Inject<SocialGUIFactory>(new SocialGUIFactory());
		socialGUIFactory.Build(gameObject, base.container);
	}

	public sealed override void BuildClassesForContext()
	{
		BuildClassesForContextGameMode();
		base.container.Build<BonusManager>();
		base.container.Build<TeamBaseAudioGui>();
		base.container.Build<SendDisconnectToServerOnWorldSwitch>();
		_networkInitialisation = new NetworkInitialisationSimulationLiteNetLib(this, _machineMotionSenderEngine);
		_networkInitialisation = base.container.Inject<NetworkInitialisationSimulationLiteNetLib>(_networkInitialisation);
		_contextNotifier.AddFrameworkDestructionListener(_networkInitialisation);
		_contextNotifier.AddFrameworkInitializationListener(_networkInitialisation);
	}

	protected override void Build()
	{
		base.Build();
	}

	protected abstract void SetUpContainerForGameMode();

	protected abstract void SetupNetworkContainerForGameMode();

	protected abstract void SetUpEntitySystemForGameMode();

	protected abstract void BuildGameObjectsForGameMode();

	protected abstract void BuildClassesForContextGameMode();

	protected BattleLeftEventObservable GetBattleLeftObserverable()
	{
		return _battleLeftObservable;
	}

	protected PlayerQuitRequestCompleteObservable GetQuitRequestCompleteObservable()
	{
		return _quitRequestCompleteObservable;
	}
}
