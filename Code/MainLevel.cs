using Achievements;
using Avatars;
using Battle;
using ChatServiceLayer;
using ChatServiceLayer.Photon;
using CustomGames;
using EnginesGUI;
using Game.RoboPass.Engines;
using Game.RoboPass.GUI;
using Game.RoboPass.GUI.Engines;
using Game.RoboPass.GUI.Observers;
using Game.Tiers.GUI;
using GameFramework;
using InputMask;
using LobbyServiceLayer;
using LobbyServiceLayer.Photon;
using Mothership;
using Mothership.Achievements;
using Mothership.Analytics;
using Mothership.Battle;
using Mothership.DailyQuest;
using Mothership.Garage.Thumbnail;
using Mothership.GarageSkins;
using Mothership.GUI;
using Mothership.GUI.Clan;
using Mothership.GUI.CustomGames;
using Mothership.GUI.Inventory;
using Mothership.GUI.Party;
using Mothership.GUI.Social;
using Mothership.ItemShop;
using Mothership.OpsRoom;
using Mothership.RobotConfiguration;
using Mothership.SinglePlayerCampaign;
using Mothership.TechTree;
using PlayMaker;
using RCNetwork.Events;
using RCNetwork.UNet.Client;
using Robocraft.GUI;
using ServerStateServiceLayer;
using ServerStateServiceLayer.Photon;
using Services;
using Services.Analytics;
using Services.TechTree;
using Services.Web.Photon;
using Simulation.Achievements;
using SinglePlayerCampaign.GUI.Mothership.Engines;
using SinglePlayerServiceLayer;
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
using Svelto.PeersLinker;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using Taunts;
using Tiers;
using UnityEngine;
using Utility;
using WebServices;

internal class MainLevel : ICompositionRoot, IUnityContextHierarchyChangedListener, IEntitySystemContext
{
	protected IContainer container;

	protected EnginesRoot _nodeEnginesRoot;

	private CommandFactory _commandFactory;

	private IGameObjectFactory _gameObjectFactory;

	private ITicker _ticker;

	private readonly IContextNotifer _contextNotifier = new ContextNotifier();

	private PeersLinker _peersLinkers;

	private EnginesRoot _legacyEnginesRoot;

	private AccountSanctions _accountSanctions;

	private MaintenanceModeController _maintenanceModeController;

	private SocialEventFeed _socialEventFeed;

	private ChatCommandsMothership _chatCommandsMothership;

	private WorldSwitchingMothershipAnalytics _worldSwitchingAnalytics;

	private AchievementCRFMasteryTracker _achievementsMasteryManager;

	private GarageBaySkinSwitcher _garageBaySwitcher;

	private IGUIInputControllerMothership _guiInputController;

	private StandardChatCommands _standardChatCommands;

	private GarageBaySkinSelectedObservable _garageBaySkinObservable;

	private RoboPassBattleSummaryObservable _roboPassBattleSummaryObservable;

	private RobopassBattleSummaryScreenFactory _robopassBSScreenFactory;

	private RobopassScreenFactory _robopassScreenFactory;

	private WorldSwitching _worldSwitching;

	private BuyRoboPassAfterBattleScreenFactory _buyRoboPassAfterBattleScreenFactory;

	private PriceChangeDialogFactory _priceChangeDialogFactory;

	private PriceChangeDialogPresenter _priceChangeDialogPresenter;

	private TLOG_RobotShopClicksTracker_Tencent _robotShopClicksTracker;

	public MainLevel()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		ClearUpServiceLayer();
		GC.Collect();
		GC.WaitForPendingFinalizers();
		Resources.UnloadUnusedAssets();
		try
		{
			Init();
			SetupContainer();
			SetupPlatformSpecificContainer();
			SetupEntitySystemLegacy();
			SetupEntitySystem();
		}
		catch (Exception ex)
		{
			RemoteLogger.Error("Crash during MainLevel initialization", ex.Message, ex.StackTrace);
			Console.LogError("Crash during MainLevel initialization " + ex);
		}
	}

	public void OnContextCreated(UnityContext contextHolder)
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
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			bool flag = false;
			IAutomaticallyBuiltEntity automaticallyBuiltEntity = componentsInChildren2[j] as IAutomaticallyBuiltEntity;
			if (automaticallyBuiltEntity != null && automaticallyBuiltEntity.InstanceCreated)
			{
				flag = true;
			}
			if (!flag)
			{
				MonoBehaviour val3 = componentsInChildren2[j] as MonoBehaviour;
				GameObject gameObject = val3.get_gameObject();
				MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
				val2.BuildEntity(gameObject.GetInstanceID(), componentsInChildren2[j].RetrieveDescriptor(), (object[])components);
			}
			if (componentsInChildren2[j] is IAutomaticallyBuiltEntity)
			{
				(componentsInChildren2[j] as IAutomaticallyBuiltEntity).InstanceCreated = true;
			}
		}
	}

	private void Init()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_peersLinkers = new PeersLinker();
	}

	private void SetupContainer()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Expected O, but got Unknown
		//IL_0f78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f82: Expected O, but got Unknown
		//IL_0f7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f87: Expected O, but got Unknown
		container = new ContextContainer(_contextNotifier);
		_ticker = new UnityTicker();
		_gameObjectFactory = new GameObjectFactory(this);
		container.Bind<IEntitySystemContext>().AsInstance<MainLevel>(this);
		container.Bind<ProfanityFilter>().AsSingle<ProfanityFilter_Tencent>();
		container.Bind<IMonoBehaviourFactory>().AsInstance<MonoBehaviourFactory>(new MonoBehaviourFactory(this));
		container.Bind<IGameObjectFactory>().AsInstance<IGameObjectFactory>(_gameObjectFactory);
		container.BindSelf<GameObjectPool>();
		container.Bind<ITicker>().AsInstance<ITicker>(_ticker);
		container.Bind<IContextNotifer>().AsInstance<IContextNotifer>(_contextNotifier);
		container.Bind<ICubeList>().AsSingle<CubeList>();
		container.Bind<IGUIComponentsWithInjectionFactory>().AsInstance<GUIComponentsWithInjectionFactory>(new GUIComponentsWithInjectionFactory(container));
		container.Bind<IComponentFactory>().AsSingle<GenericComponentFactory>();
		container.BindSelf<ClanComponentFactory>();
		container.BindSelf<FriendComponentFactory>();
		container.BindSelf<ClanPopupMenuController>();
		container.Bind<IContextSensitiveXPRefresher>().AsSingle<ContextSensitiveXPRefresher>();
		container.BindSelf<FriendPopupMenuController>();
		container.BindSelf<PartyPopupMenuController>();
		container.BindSelf<CustomGamePopupMenuController>();
		container.BindSelf<GameModeChoiceFilterPresenter>();
		container.BindSelf<MapChoiceFilterPresenter>();
		container.BindSelf<CreateClanLayoutFactory>();
		container.Bind<SearchClanLayoutFactory>().AsInstance<SearchClanLayoutFactory>(new SearchClanLayoutFactory(container));
		container.Bind<YourClanLayoutFactory>().AsInstance<YourClanLayoutFactory>(new YourClanLayoutFactory(container));
		container.BindSelf<FriendListLayoutFactory>();
		container.BindSelf<ClanInvitesLayoutFactory>();
		container.BindSelf<CustomGameOptionsFactory>();
		container.BindSelf<NotificationsBoxLayoutFactory>();
		container.BindSelf<PlayerLevelInfoLayoutFactory>();
		container.BindSelf<InvitePlayerToPartyGuiFactory>();
		container.BindSelf<TopBarDisplayFactory>();
		container.BindSelf<ClanSeasonRewardScreenLayoutFactory>();
		container.BindSelf<SocialGUIFactory>();
		container.BindSelf<DragAndDropGUIFactory>();
		container.BindSelf<RobotShopFactory>();
		container.BindSelf<RobotShopTransactionFactory>();
		container.BindSelf<CustomGamePartyGUIFactory>();
		container.BindSelf<PopupContextMenuFactory>();
		container.BindSelf<BattleExperienceScreenFactory>();
		container.BindSelf<ControlsChangedObserver>();
		container.Bind<IMothershipPropPresenter>().AsSingle<MothershipPropPresenter>();
		container.Bind<IEditorCubeFactory>().AsSingle<EditorCubeFactory>();
		container.Bind<ICubeFactory>().AsSingle<EditorCubeFactory>();
		container.Bind<IMachineBuilder>().AsSingle<MachineBuilder>();
		container.Bind<IMachineMap>().AsSingle<MachineMap>();
		_commandFactory = new CommandFactory(container);
		container.Bind<ICommandFactory>().AsInstance<CommandFactory>(_commandFactory);
		container.Bind<ICubeHolder>().AsSingle<CubeHolder>();
		container.Bind<ICursorMode>().AsSingle<CursorMode>();
		container.Bind<ICubeInventory>().AsSingle<CubeInventory>();
		container.Bind<ICubePrerequisites>().AsSingle<CubePrerequisites>();
		container.Bind<ICubesData>().AsSingle<CubesData>();
		container.Bind<ICubeInventoryData>().AsSingle<CubeInventoryData>();
		container.Bind<ICurrenciesTracker>().AsSingle<CurrenciesTracker>();
		container.Bind<IDispatchWorldSwitching>().AsSingle<WorldSwitching>();
		container.Bind<IPauseManager>().AsSingle<PauseManagerWorkshop>();
		container.Bind<ITauntMaskHelper>().AsSingle<TauntMaskHelper>();
		container.BindSelf<CustomGameStateObservable>();
		container.Bind<CustomGameStateObserver>().AsInstance<CustomGameStateObserver>(new CustomGameStateObserver(container.Build<CustomGameStateObservable>()));
		container.BindSelf<CustomGameGameModeObservable>();
		container.Bind<CustomGameGameModeObserver>().AsInstance<CustomGameGameModeObserver>(new CustomGameGameModeObserver(container.Build<CustomGameGameModeObservable>()));
		container.BindSelf<CosmeticCreditsObservable>();
		container.Bind<CosmeticCreditsObserver>().AsInstance<CosmeticCreditsObserver>(new CosmeticCreditsObserver(container.Build<CosmeticCreditsObservable>()));
		container.BindSelf<DisableChatInputObservable>();
		container.Bind<DisableChatInputObserver>().AsInstance<DisableChatInputObserver>(new DisableChatInputObserver(container.Build<DisableChatInputObservable>()));
		container.BindSelf<CubePrerequisitesFailedObservable>();
		container.Bind<CubePrerequisitesFailedObserver>().AsInstance<CubePrerequisitesFailedObserver>(new CubePrerequisitesFailedObserver(container.Build<CubePrerequisitesFailedObservable>()));
		container.BindSelf<SwitchingToTestModeObservable>();
		container.Bind<SwitchingToTestModeObserver>().AsInstance<SwitchingToTestModeObserver>(new SwitchingToTestModeObserver(container.Build<SwitchingToTestModeObservable>()));
		container.Bind<ICPUPower>().AsSingle<CPUPower>();
		container.Bind<IMachineTeleportAudio>().AsSingle<MachineTeleportAudioFabric>();
		TutorialTipObservable tutorialTipObservable = new TutorialTipObservable();
		InvalidPlacementObservable invalidPlacementObservable = new InvalidPlacementObservable();
		BuildModeShortcutHintsObserveable buildModeShortcutHintsObserveable = new BuildModeShortcutHintsObserveable();
		DragAndDropGUIEventObservable dragAndDropGUIEventObservable = new DragAndDropGUIEventObservable();
		container.Bind<ITutorialController>().AsInstance<MothershipTutorialController>(new MothershipTutorialController(tutorialTipObservable, buildModeShortcutHintsObserveable));
		container.Bind<ITutorialCubePlacementController>().AsSingle<TutorialCubePlacementController>();
		SocialEventListenerFactory socialEventListenerFactory = new SocialEventListenerFactory();
		SocialEventRegistry socialEventRegistry = new SocialEventRegistry(socialEventListenerFactory);
		container.Bind<ISocialEventContainerFactory>().AsInstance<SocialEventContainerFactory>(new SocialEventContainerFactory(socialEventRegistry));
		container.Bind<ISocialRequestFactory>().AsSingle<SocialRequestFactory>();
		LobbyEventListenerFactory lobbyEventListenerFactory = new LobbyEventListenerFactory();
		LobbyEventRegistry lobbyEventRegistry = new LobbyEventRegistry(lobbyEventListenerFactory);
		container.Bind<ILobbyEventContainerFactory>().AsInstance<LobbyEventContainerFactory>(new LobbyEventContainerFactory(lobbyEventRegistry));
		container.Bind<ILobbyRequestFactory>().AsSingle<LobbyRequestFactory>();
		ServerStateEventListenerFactory serverStateEventListenerFactory = new ServerStateEventListenerFactory();
		ServerStateEventRegistry serverStateEventRegistry = new ServerStateEventRegistry(serverStateEventListenerFactory);
		container.Bind<IServerStateEventContainerFactory>().AsInstance<ServerStateEventContainerFactory>(new ServerStateEventContainerFactory(serverStateEventRegistry));
		container.Bind<IServerStateRequestFactory>().AsSingle<ServerStateRequestFactory>();
		SinglePlayerEventListenerFactory singlePlayerEventListenerFactory = new SinglePlayerEventListenerFactory();
		SinglePlayerEventRegistry singlePlayerEventRegistry = new SinglePlayerEventRegistry(singlePlayerEventListenerFactory);
		container.Bind<ISinglePlayerEventContainerFactory>().AsInstance<SinglePlayerEventContainerFactory>(new SinglePlayerEventContainerFactory(singlePlayerEventRegistry));
		container.Bind<ISinglePlayerRequestFactory>().AsSingle<SinglePlayerRequestFactory>();
		ChatEventListenerFactory chatEventListenerFactory = new ChatEventListenerFactory();
		ChatEventRegistry chatEventRegistry = new ChatEventRegistry(chatEventListenerFactory);
		container.Bind<IChatEventContainerFactory>().AsInstance<ChatEventContainerFactory>(new ChatEventContainerFactory(chatEventRegistry));
		container.Bind<IChatRequestFactory>().AsSingle<ChatRequestFactory>();
		container.Bind<IAnalyticsRequestFactory>().AsSingle<AnalyticsRequestFactory>();
		container.Bind<IRobotShopController>().AsSingle<RobotShopController>();
		container.Bind<ChatPresenter>().AsSingle<ChatPresenterMothership>();
		container.Bind<CustomGamePartyGUIController>().AsSingle<CustomGamePartyGUIController>();
		container.BindSelf<ChatChannelCommands>();
		container.Bind<IPlayMakerStateMachineBridge>().AsSingle<PlayMakerStateMachineBridge>();
		InvalidPlacementObserver popupMessageObserver = new InvalidPlacementObserver(invalidPlacementObservable);
		container.Bind<PopupMessagePresenter>().AsInstance<PopupMessagePresenter>(new PopupMessagePresenter(popupMessageObserver, new TutorialTipObserver(tutorialTipObservable)));
		container.Bind<BuildModeShortcutHintsPresenter>().AsInstance<BuildModeShortcutHintsPresenter>(new BuildModeShortcutHintsPresenter(new BuildModeShortcutHintsObserver(buildModeShortcutHintsObserveable)));
		container.BindSelf<CubeSelectHighlighter>();
		container.Bind<DragAndDropGUIEventObserver>().AsInstance<DragAndDropGUIEventObserver>(new DragAndDropGUIEventObserver(dragAndDropGUIEventObservable));
		container.BindSelf<RobotDimensionChangedObservable>();
		container.Bind<RobotDimensionChangedObserver>().AsInstance<RobotDimensionChangedObserver>(new RobotDimensionChangedObserver(container.Build<RobotDimensionChangedObservable>()));
		container.BindSelf<GarageChangedObservable>();
		container.Bind<GarageChangedObserver>().AsInstance<GarageChangedObserver>(new GarageChangedObserver(container.Build<GarageChangedObservable>()));
		container.Bind<IRobotShopController>().AsSingle<RobotShopController>();
		container.Bind<IGUIInputControllerMothership>().AsSingle<GUIInputController>();
		container.Bind<IGUIInputController>().AsSingle<GUIInputController>();
		container.Bind<IAutoSaveController>().AsSingle<AutoSaveController>();
		container.Bind<BattleParameters>().AsSingle<BattleParametersMothership>();
		container.Bind<BattlePlayers>().AsSingle<BattlePlayersMothership>();
		container.BindSelf<BattlePlayersMothership>();
		container.Bind<BattleTimer>().AsSingle<BattleTimerMothership>();
		container.Bind<ILobbyPlayerListPresenter>().AsSingle<LobbyPlayerListPresenterMothership>();
		container.BindSelf<SellRobotPresenter>();
		container.BindSelf<UnlockCubeTypePresenter>();
		container.BindSelf<ChatClientProvider>();
		container.BindSelf<ChatWarningDialogue>();
		container.Bind<IgnoreList>().AsSingle<IgnoreListMothership>();
		container.BindSelf<CurrentCubeSelectorCategory>();
		container.BindSelf<WeaponTypeEditModeCount>();
		container.BindSelf<LeagueBadgesEditModeCount>();
		container.BindSelf<CPUExceededDisplay>();
		container.BindSelf<SpecialItems>();
		container.BindSelf<ProfileUpdateNotification>();
		container.BindSelf<MachineMover>();
		container.BindSelf<CubeBoundsCache>();
		container.BindSelf<RobotNudgeManager>();
		container.BindSelf<AdvancedRobotEditSettings>();
		container.BindSelf<EnterBattleChecker>();
		container.BindSelf<CubeSelectorDisplay>();
		container.BindSelf<GhostCubeController>();
		container.BindSelf<CubeSelectorPresenter>();
		container.BindSelf<ClanController>();
		container.BindSelf<FriendController>();
		container.BindSelf<UploadAvatarController>();
		container.BindSelf<HUDCPUPowerGaugePresenter>();
		container.BindSelf<HUDSpeedPreseneter>();
		container.BindSelf<HUDDamageBoostPresenter>();
		container.BindSelf<HUDMassPresenter>();
		container.BindSelf<HUDCosmeticCPUPresenter>();
		container.BindSelf<HUDHealthPresenter>();
		container.BindSelf<HUDRobotInfoPresenter>();
		container.BindSelf<HUDPlayerLevelPresenter>();
		container.BindSelf<HUDHiderMothershipPresenter>();
		container.BindSelf<SettingsDisplay>();
		container.BindSelf<InputController>();
		container.BindSelf<AvatarHiderController>();
		container.BindSelf<QuitListenerManager>();
		container.BindSelf<ClanSeasonRewardScreenController>();
		container.BindSelf<DevMessagePresenter>();
		container.BindSelf<ControlsDisplay>();
		container.BindSelf<GaragePresenter>();
		container.BindSelf<GarageExtraButtonsPresenter>();
		container.BindSelf<GarageSlotOrderPresenter>();
		container.BindSelf<SlotOrderSaveController>();
		container.BindSelf<GarageSlotsPresenter>();
		container.BindSelf<GarageDisplay>();
		container.BindSelf<BuildDisplay>();
		container.BindSelf<EnterPlanetDialogueController>();
		container.BindSelf<BattleCountdownScreenController>();
		container.BindSelf<WorldSwitching>();
		container.BindSelf<LobbyCountdown>();
		container.BindSelf<AFKBlockTimer>();
		container.BindSelf<AFKWarningClientCommand>();
		container.BindSelf<ChatSettings>();
		container.BindSelf<ChatCommands>();
		container.BindSelf<ChatChannelContainer>();
		container.BindSelf<PrivateChat>();
		container.BindSelf<PremiumMembership>();
		container.BindSelf<PremiumMembershipActivatedMediator>();
		container.BindSelf<PremiumMembershipExpiredMediator>();
		container.BindSelf<BlurEffectController>();
		container.BindSelf<PartyAlertBarController>();
		container.BindSelf<FriendInviteManager>();
		container.BindSelf<MasterVolumeController>();
		container.BindSelf<MouseSettings>();
		container.BindSelf<CameraSettings>();
		container.BindSelf<LegacyControlSettings>();
		container.BindSelf<LocalisationSettings>();
		container.BindSelf<RobotNameWriterPresenter>();
		container.BindSelf<GenericInfoDisplay>();
		container.BindSelf<ThumbnailTriggerer>();
		container.BindSelf<RobotShopController>();
		container.BindSelf<RobotShopDisplay>();
		container.BindSelf<RobotShopCommunityController>();
		container.BindSelf<RobotShopTransactionController>();
		container.BindSelf<RobotShopObserver>();
		container.BindSelf<LoadingIconPresenter>();
		container.BindSelf<ThumbnailManager>();
		container.BindSelf<RobotThumbnailFetcher>();
		container.BindSelf<RobotShopSubmissionController>();
		container.BindSelf<RobotShopRatingController>();
		container.BindSelf<MachineEditorBuilder>();
		container.BindSelf<MachineEditorGraphUpdater>();
		container.BindSelf<MachineColorUpdater>();
		container.BindSelf<CameraPreview>();
		container.BindSelf<GarageCameraOrientationController>();
		container.BindSelf<NormalBattleAvailability>();
		container.BindSelf<TeamDeathMatchAvailability>();
		container.BindSelf<GameObjectPool>();
		container.BindSelf<MachineEditorBatcher>();
		container.BindSelf<MachineUpdater>();
		container.BindSelf<MirrorMode>();
		container.BindSelf<CubeRaycastInfo>();
		container.BindSelf<SocialSettings>();
		container.BindSelf<DesiredGameMode>();
		container.BindSelf<CustomGameScreenController>();
		container.BindSelf<ChatAudio>();
		container.Bind<MachineEditorCollisionChecker>().AsSingle<MachineEditorCollisionChecker>();
		container.BindSelf<AvatarSelectionPresenter>();
		container.BindSelf<AvatarPresenterLocalPlayer>();
		container.BindSelf<WorldSwitchLoadingDisplay>();
		container.BindSelf<PartyGUIController>();
		container.Bind<IPartyIconController>().ToProvider<PartyIconController>(new MultiProvider<PartyIconController>());
		container.Bind<IPartyInvitationDialogController>().ToProvider<PartyInvitationDialogController>(new MultiProvider<PartyInvitationDialogController>());
		container.Bind<IDragAndDropGUIBehaviourController>().AsInstance<DragAndDropGUIBehaviourController>(new DragAndDropGUIBehaviourController(dragAndDropGUIEventObservable));
		container.BindSelf<BattleExperiencePresenter>();
		container.BindSelf<BattleExperienceLevelPresenter>();
		container.BindSelf<PaintToolPresenter>();
		container.BindSelf<CurrentToolMode>();
		container.BindSelf<PaintColorSelectorDisplay>();
		container.BindSelf<PaintFillController>();
		container.BindSelf<CapFrameRateSettings>();
		container.BindSelf<AccountSanctionsMothership>();
		container.BindSelf<WeaponOrderManager>();
		container.BindSelf<ItemDescriptorSpriteUtility>();
		container.BindSelf<WeaponReorderDisplay>();
		container.BindSelf<BundleAwardController>();
		container.BindSelf<CentreRobot>();
		container.BindSelf<BuildModeHUDVisibility>();
		container.BindSelf<PurchaseRefresher>();
		container.BindSelf<PresetAvatarMapProvider>();
		container.BindSelf<AvatarAvailableObservable>();
		container.Bind<AvatarAvailableObserver>().AsInstance<AvatarAvailableObserver>(new AvatarAvailableObserver(container.Build<AvatarAvailableObservable>()));
		container.BindSelf<CrfItemListLoader>();
		container.BindSelf<RobotCostCalculator>();
		container.BindSelf<GarageOptionsPresenter>();
		container.BindSelf<MaxCosmeticCPUChangedObservable>();
		container.Bind<MaxCosmeticCPUChangedObserver>().AsInstance<MaxCosmeticCPUChangedObserver>(new MaxCosmeticCPUChangedObserver(container.Build<MaxCosmeticCPUChangedObservable>()));
		container.Bind<LobbyPresenter>().AsInstance<LobbyPresenter>(new LobbyPresenter());
		container.BindSelf<LobbyView>();
		container.BindSelf<BattleFoundObserver>();
		container.BindSelf<TutorialButtonPresenter>();
		container.BindSelf<BrawlButtonPresenter>();
		container.BindSelf<BrawlDetailsPresenter>();
		container.BindSelf<MothershipReadyObservable>();
		container.Bind<MothershipReadyObserver>().AsInstance<MothershipReadyObserver>(new MothershipReadyObserver(container.Build<MothershipReadyObservable>()));
		container.BindSelf<BuildInputLock>();
		BindContextSpecificItems(invalidPlacementObservable);
		container.BindSelf<NetworkClientPool>();
		container.Bind<INetworkEventManagerClient>().AsSingle<NetworkEventManagerClientLiteNetLib>();
		container.Bind<INetworkInitialisationTestClient>().AsSingle<NetworkInitialisationTestClientLiteNetLib>();
		container.Bind<NetworkEventRegistrationMothership>().AsSingle<NetworkInitialisationMothershipLiteNetLib>();
		container.BindSelf<TestConnection>();
		container.BindSelf<LocalisationWrapper>();
		container.BindSelf<BrawlButtonFactory>();
		container.BindSelf<BrawlDetailsScreenFactory>();
		container.Bind<BattleLoadProgress>().AsSingle<BattleLoadProgressMothership>();
		container.BindSelf<InitialLoginAnalytics>();
		container.BindSelf<TauntsMothershipController>();
		container.BindSelf<ReconnectPresenter>();
		container.BindSelf<TauntsMothershipController>();
		container.BindSelf<PlayerLevelNeedRefreshObservable>();
		container.BindSelf<DailyQuestController>();
		container.BindSelf<HUDBuildModeHintsPresenter>();
		container.BindSelf<SetBuildModeHintsAnchorsObserverable>();
		container.Bind<SetBuildModeHintsAnchorsObserver>().AsInstance<SetBuildModeHintsAnchorsObserver>(new SetBuildModeHintsAnchorsObserver(container.Build<SetBuildModeHintsAnchorsObserverable>()));
		_garageBaySkinObservable = new GarageBaySkinSelectedObservable();
		container.Bind<GarageBaySkinSelectedObservable>().AsInstance<GarageBaySkinSelectedObservable>(_garageBaySkinObservable);
		container.Bind<GarageBaySkinSelectedObserver>().AsInstance<GarageBaySkinSelectedObserver>(new GarageBaySkinSelectedObserver(container.Build<GarageBaySkinSelectedObservable>()));
		container.BindSelf<GarageBaySkinNotificationObservable>();
		container.Bind<GarageBaySkinNotificationObserver>().AsInstance<GarageBaySkinNotificationObserver>(new GarageBaySkinNotificationObserver(container.Build<GarageBaySkinNotificationObservable>()));
		container.BindSelf<RobotSanctionController>();
		container.BindSelf<BuildHistoryManager>();
		container.BindSelf<NewRobotOptionsPresenter>();
		container.BindSelf<PrebuiltRobotPresenter>();
		container.BindSelf<PrebuiltRobotBuilder>();
		container.BindSelf<DailyQuestPresenter>();
		container.BindSelf<QuestProgressionPresenter>();
		container.BindSelf<TechPointsTracker>();
		container.BindSelf<TechPointsPresenter>();
		container.BindSelf<TierProgressionRewardPresenter>();
		_nodeEnginesRoot = new EnginesRoot(new UnitySumbmissionEntityViewScheduler());
		container.Bind<IEntityFactory>().AsInstance<IEntityFactory>(_nodeEnginesRoot.GenerateEntityFactory());
		container.BindSelf<ShowCampaignCompleteScreenEngine>();
		container.BindSelf<PurchaseConfirmedController>();
		container.BindSelf<DailyQuestsObservable>();
		container.BindSelf<DailyQuestAnalytics>();
		container.Bind<IRealMoneyStoreCardController>().ToProvider<RealMoneyStoreCardController>(new MultiProvider<RealMoneyStoreCardController>());
		container.Bind<IRealMoneyStoreRoboPassPossibleItemController>().ToProvider<RealMoneyStoreRoboPassPossibleItemController>(new MultiProvider<RealMoneyStoreRoboPassPossibleItemController>());
		container.BindSelf<RobotConfigurationDataSource>();
		container.BindSelf<BrowserURLChangedObservable>();
		container.BindSelf<BrowserClosedObservable>();
		PurchaseRequestFactory purchaseRequestFactory = new PurchaseRequestFactory(_nodeEnginesRoot.GenerateEntityFactory(), container.Build<IGUIInputControllerMothership>());
		RealMoneyStorePresenter realMoneyStorePresenter = new RealMoneyStorePresenter(purchaseRequestFactory, container.Build<IAnalyticsRequestFactory>());
		BuyPremiumAfterBattlePresenter buyPremiumAfterBattlePresenter = new BuyPremiumAfterBattlePresenter(purchaseRequestFactory);
		BuyRoboPassAfterBattlePresenter buyRoboPassAfterBattlePresenter = new BuyRoboPassAfterBattlePresenter(purchaseRequestFactory);
		container.Bind<BuyPremiumAfterBattlePresenter>().AsInstance<BuyPremiumAfterBattlePresenter>(buyPremiumAfterBattlePresenter);
		container.Bind<BuyRoboPassAfterBattlePresenter>().AsInstance<BuyRoboPassAfterBattlePresenter>(buyRoboPassAfterBattlePresenter);
		container.Bind<RealMoneyStorePresenter>().AsInstance<RealMoneyStorePresenter>(realMoneyStorePresenter);
		_roboPassBattleSummaryObservable = new RoboPassBattleSummaryObservable();
		RoboPassBattleSummaryObserver roboPassBattleSummaryObserver = new RoboPassBattleSummaryObserver(_roboPassBattleSummaryObservable);
		container.Bind<RoboPassBattleSummaryObserver>().AsInstance<RoboPassBattleSummaryObserver>(roboPassBattleSummaryObserver);
		container.BindSelf<ReloadRobopassObservable>();
		RealMoneyStoreDataSource realMoneyStoreDataSource = new RealMoneyStoreDataSource(container.Build<LoadingIconPresenter>());
		container.Bind<RealMoneyStoreDataSource>().AsInstance<RealMoneyStoreDataSource>(realMoneyStoreDataSource);
		_priceChangeDialogPresenter = new PriceChangeDialogPresenter();
		container.Bind<PriceChangeDialogPresenter>().AsInstance<PriceChangeDialogPresenter>(_priceChangeDialogPresenter);
	}

	private void SetupPlatformSpecificContainer()
	{
		container.BindSelf<TLOG_LeftEditModeTracker_Tencent>();
		container.BindSelf<CreatedNewRobotObservable_Tencent>();
		container.Bind<CreatedNewRobotObserver_Tencent>().AsInstance<CreatedNewRobotObserver_Tencent>(new CreatedNewRobotObserver_Tencent(container.Build<CreatedNewRobotObservable_Tencent>()));
		container.Bind<IMultiAvatarLoader>().AsSingle<MultiAvatarLoader_Tencent>();
		container.Bind<IAchievementManager>().AsSingle<TencentAchievementManager>();
		container.Bind<IReviewRequestController>().AsSingle<ReviewRequestController_Tencent>();
		container.BindSelf<AwardedItemsController>();
	}

	public virtual void BindContextSpecificItems(InvalidPlacementObservable invalidPlacementObservable)
	{
		container.Bind<ICubeLauncherPermission>().AsInstance<CubeLauncherPermission>(new CubeLauncherPermission(invalidPlacementObservable));
		container.BindSelf<InitialMothershipGUIFlow>();
		container.Bind<IServiceRequestFactory>().AsSingle<WebStorageRequestFactoryDefault>();
		container.Bind<IPauseMenuController>().AsSingle<PauseMenuControllerMothership>();
		container.Bind<IGhostCubeVisibilityChecker>().AsSingle<GhostCubeVisibilityChecker>();
		container.Bind<ICubeSelectVisibilityChecker>().AsSingle<CubeSelectVisibilityCheckerNormal>();
		container.Bind<IInputActionMask>().AsSingle<InputActionMaskNormal>();
		container.Bind<IDummyTestModeScreenDisplay>().AsInstance<DummyTestModeScreenDisplay>(new DummyTestModeScreenDisplay());
	}

	public virtual void BuildContextSpecificItems()
	{
		container.Build<InitialMothershipGUIFlow>();
		container.Build<InitialLoginAnalytics>();
	}

	public virtual ITopBarDisplay BuildContextSpecificTopBar()
	{
		TopBarDisplay topBarDisplay = new TopBarDisplay();
		container.Bind<ITopBarDisplay>().AsInstance<TopBarDisplay>(topBarDisplay);
		container.Inject<TopBarDisplay>(topBarDisplay);
		return topBarDisplay;
	}

	private void SetupEntitySystem()
	{
		IEntityFactory entityFactory = _nodeEnginesRoot.GenerateEntityFactory();
		IAnalyticsRequestFactory analyticsRequestFactory = container.Build<IAnalyticsRequestFactory>();
		ICubeInventory cubeInventory = container.Build<ICubeInventory>();
		ICurrenciesTracker currenciesTracker = container.Build<ICurrenciesTracker>();
		IGUIInputController guiInputController = container.Build<IGUIInputController>();
		LoadingIconPresenter loadingIconPresenter = container.Build<LoadingIconPresenter>();
		PremiumMembership premiumMembership = container.Build<PremiumMembership>();
		IServiceRequestFactory serviceRequestFactory = container.Build<IServiceRequestFactory>();
		LayoutAdjustmentToScreenConfigEngine layoutAdjustmentToScreenConfigEngine = container.Inject<LayoutAdjustmentToScreenConfigEngine>(new LayoutAdjustmentToScreenConfigEngine());
		RetargetableSpriteAdjustToScreenConfigEngine retargetableSpriteAdjustToScreenConfigEngine = container.Inject<RetargetableSpriteAdjustToScreenConfigEngine>(new RetargetableSpriteAdjustToScreenConfigEngine());
		RetargetableParticleAdjustToScreenConfigEngine retargetableParticleAdjustToScreenConfigEngine = container.Inject<RetargetableParticleAdjustToScreenConfigEngine>(new RetargetableParticleAdjustToScreenConfigEngine());
		_nodeEnginesRoot.AddEngine(layoutAdjustmentToScreenConfigEngine);
		_nodeEnginesRoot.AddEngine(retargetableSpriteAdjustToScreenConfigEngine);
		_nodeEnginesRoot.AddEngine(retargetableParticleAdjustToScreenConfigEngine);
		IDataSource<Dictionary<string, TechTreeItemData>> dataSource = container.Inject<TechTreeDataSource>(new TechTreeDataSource());
		TechTreeItemsFactory itemsFactory = new TechTreeItemsFactory(_nodeEnginesRoot.GenerateEntityFactory());
		TechTreeDisplayEngine techTreeDisplayEngine = container.Inject<TechTreeDisplayEngine>(new TechTreeDisplayEngine(itemsFactory, dataSource, _guiInputController));
		_nodeEnginesRoot.AddEngine(techTreeDisplayEngine);
		TechTreeNavigationEngine techTreeNavigationEngine = new TechTreeNavigationEngine();
		_nodeEnginesRoot.AddEngine(techTreeNavigationEngine);
		TechTreeZoomEngine techTreeZoomEngine = new TechTreeZoomEngine();
		_nodeEnginesRoot.AddEngine(techTreeZoomEngine);
		_contextNotifier.AddFrameworkDestructionListener(techTreeZoomEngine);
		TechTreeUnlockItemEngine techTreeUnlockItemEngine = container.Inject<TechTreeUnlockItemEngine>(new TechTreeUnlockItemEngine(loadingIconPresenter));
		_nodeEnginesRoot.AddEngine(techTreeUnlockItemEngine);
		_guiInputController.AddDisplayScreens(new IGUIDisplay[1]
		{
			techTreeDisplayEngine
		});
		StatsHintPopupEngine statsHintPopupEngine = container.Inject<StatsHintPopupEngine>(new StatsHintPopupEngine(container.Build<ICubeInventoryData>(), container.Build<ICubeList>(), serviceRequestFactory, guiInputController));
		_nodeEnginesRoot.AddEngine(statsHintPopupEngine);
		_contextNotifier.AddFrameworkInitializationListener(statsHintPopupEngine);
		ItemShopDataSource dataSource2 = container.Inject<ItemShopDataSource>(new ItemShopDataSource());
		ItemShopGUIFactory factory = container.Inject<ItemShopGUIFactory>(new ItemShopGUIFactory());
		ItemShopDisplayEngine itemShopDisplayEngine = new ItemShopDisplayEngine(dataSource2, factory, loadingIconPresenter);
		ItemShopBundleLayoutEngine itemShopBundleLayoutEngine = new ItemShopBundleLayoutEngine(dataSource2, loadingIconPresenter, analyticsRequestFactory);
		ItemShopPurchaseEngine itemShopPurchaseEngine = container.Inject<ItemShopPurchaseEngine>(new ItemShopPurchaseEngine());
		ItemShopCTAEngine itemShopCTAEngine = new ItemShopCTAEngine(dataSource2, analyticsRequestFactory);
		_nodeEnginesRoot.AddEngine(itemShopDisplayEngine);
		_nodeEnginesRoot.AddEngine(itemShopBundleLayoutEngine);
		_nodeEnginesRoot.AddEngine(itemShopPurchaseEngine);
		_nodeEnginesRoot.AddEngine(itemShopCTAEngine);
		RobotConfigurationGUIFactory factory2 = container.Inject<RobotConfigurationGUIFactory>(new RobotConfigurationGUIFactory());
		RobotConfigurationDisplayEngine robotConfigurationDisplayEngine = new RobotConfigurationDisplayEngine(container.Build<RobotConfigurationDataSource>(), factory2, loadingIconPresenter, _commandFactory, serviceRequestFactory, _garageBaySkinObservable);
		_nodeEnginesRoot.AddEngine(robotConfigurationDisplayEngine);
		_guiInputController.AddDisplayScreens(new IGUIDisplay[1]
		{
			robotConfigurationDisplayEngine
		});
		OpsRoomDisplayEngine opsRoomDisplayEngine = container.Inject<OpsRoomDisplayEngine>(new OpsRoomDisplayEngine(_guiInputController, serviceRequestFactory, loadingIconPresenter, container.Build<DailyQuestController>()));
		_nodeEnginesRoot.AddEngine(opsRoomDisplayEngine);
		_guiInputController.AddDisplayScreens(new IGUIDisplay[1]
		{
			opsRoomDisplayEngine
		});
		_nodeEnginesRoot.AddEngine(new OpsRoomShowTechTreeCTAEngine(container.Build<TechPointsTracker>(), dataSource));
		_nodeEnginesRoot.AddEngine(new OpsRoomShowQuestsCTAEngine(container.Build<DailyQuestController>(), new DailyQuestsObserver(container.Build<DailyQuestsObservable>())));
		_nodeEnginesRoot.AddEngine(new TopBarShowOpsRoomCTAEngine());
		_nodeEnginesRoot.AddEngine(new GameModePreferencesScreenEngine(serviceRequestFactory, loadingIconPresenter));
		IServerStateEventContainerFactory serverStateEventContainerFactory = container.Build<IServerStateEventContainerFactory>();
		_nodeEnginesRoot.AddEngine(new SinglePlayerCampaignLayoutEngine(container.Build<ISinglePlayerRequestFactory>(), loadingIconPresenter, serviceRequestFactory, _commandFactory, _guiInputController, serverStateEventContainerFactory));
		_nodeEnginesRoot.AddEngine(container.Build<ShowCampaignCompleteScreenEngine>());
		TierRobotRankingWidgetsEngine tierRobotRankingWidgetsEngine = new TierRobotRankingWidgetsEngine(container.Build<ICPUPower>(), serviceRequestFactory, loadingIconPresenter);
		_nodeEnginesRoot.AddEngine(tierRobotRankingWidgetsEngine);
		_contextNotifier.AddFrameworkInitializationListener(tierRobotRankingWidgetsEngine);
		RobotRankingEngine robotRankingEngine = new RobotRankingEngine(container.Build<IMachineMap>());
		_nodeEnginesRoot.AddEngine(robotRankingEngine);
		_contextNotifier.AddFrameworkInitializationListener(robotRankingEngine);
		_contextNotifier.AddFrameworkDestructionListener(robotRankingEngine);
		_robopassScreenFactory = new RobopassScreenFactory(entityFactory, _gameObjectFactory);
		_robopassBSScreenFactory = new RobopassBattleSummaryScreenFactory(entityFactory, _gameObjectFactory);
		ReloadRobopassObserver reloadRobopassObserver = new ReloadRobopassObserver(container.Build<ReloadRobopassObservable>());
		RoboPassEngine roboPassEngine = new RoboPassEngine(cubeInventory, currenciesTracker, serviceRequestFactory, reloadRobopassObserver);
		_nodeEnginesRoot.AddEngine(roboPassEngine);
		_contextNotifier.AddFrameworkDestructionListener(roboPassEngine);
		RoboPassMothershipScreenDisplayEngine roboPassMothershipScreenDisplayEngine = new RoboPassMothershipScreenDisplayEngine(guiInputController, serviceRequestFactory, premiumMembership, _robopassScreenFactory);
		_nodeEnginesRoot.AddEngine(roboPassMothershipScreenDisplayEngine);
		RoboPassRewardsDisplayEngine roboPassRewardsDisplayEngine = new RoboPassRewardsDisplayEngine(cubeInventory, currenciesTracker, _robopassScreenFactory, serviceRequestFactory);
		_nodeEnginesRoot.AddEngine(roboPassRewardsDisplayEngine);
		RoboPassButtonsOnClickGoToStoreEngine roboPassButtonsOnClickGoToStoreEngine = new RoboPassButtonsOnClickGoToStoreEngine(analyticsRequestFactory, guiInputController);
		_nodeEnginesRoot.AddEngine(roboPassButtonsOnClickGoToStoreEngine);
		RoboPassSeasonTimerDisplayEngine roboPassSeasonTimerDisplayEngine = new RoboPassSeasonTimerDisplayEngine();
		_nodeEnginesRoot.AddEngine(roboPassSeasonTimerDisplayEngine);
		RoboPassBattleSummaryScreenDisplayEngine roboPassBattleSummaryScreenDisplayEngine = new RoboPassBattleSummaryScreenDisplayEngine(_worldSwitching, serviceRequestFactory, _robopassBSScreenFactory, _roboPassBattleSummaryObservable, _guiInputController, reloadRobopassObserver, analyticsRequestFactory);
		_nodeEnginesRoot.AddEngine(roboPassBattleSummaryScreenDisplayEngine);
		_contextNotifier.AddFrameworkDestructionListener(roboPassBattleSummaryScreenDisplayEngine);
		RoboPassAnalyticsEngine roboPassAnalyticsEngine = new RoboPassAnalyticsEngine(container.Build<IAnalyticsRequestFactory>(), container.Build<LoadingIconPresenter>(), currenciesTracker, reloadRobopassObserver);
		_nodeEnginesRoot.AddEngine(roboPassAnalyticsEngine);
		_contextNotifier.AddFrameworkDestructionListener(roboPassAnalyticsEngine);
	}

	private void SetupEntitySystemLegacy()
	{
		_legacyEnginesRoot = new EnginesRoot();
		ICursorMode cursorMode = container.Build<ICursorMode>();
		_guiInputController = container.Build<IGUIInputControllerMothership>();
		EnterPlanetDialogueController enterPlanetDialogueController = container.Build<EnterPlanetDialogueController>();
		_guiInputController.AddDisplayScreens(new IGUIDisplay[30]
		{
			container.Build<IPauseMenuController>() as IGUIDisplay,
			container.Build<CubeSelectorDisplay>(),
			container.Build<SettingsDisplay>(),
			container.Build<ControlsDisplay>(),
			container.Build<BuildDisplay>(),
			container.Build<GarageDisplay>(),
			container.Build<RobotShopDisplay>(),
			container.Build<BattleCountdownScreenController>(),
			enterPlanetDialogueController,
			container.Build<IDummyTestModeScreenDisplay>() as IGUIDisplay,
			container.Build<GenericInfoDisplay>(),
			container.Build<CustomGameScreenController>(),
			container.Build<PaintColorSelectorDisplay>(),
			container.Build<WeaponReorderDisplay>(),
			container.Build<BundleAwardController>(),
			container.Build<ITutorialController>() as IGUIDisplay,
			container.Build<AvatarSelectionPresenter>(),
			container.Build<WorldSwitchLoadingDisplay>(),
			container.Build<ClanSeasonRewardScreenController>(),
			container.Build<BattleExperiencePresenter>(),
			container.Build<BrawlDetailsPresenter>(),
			container.Inject<PrebuiltRobotDisplay>(new PrebuiltRobotDisplay()),
			container.Inject<NewRobotOptionsDisplay>(new NewRobotOptionsDisplay()),
			container.Inject<DailyQuestDisplay>(new DailyQuestDisplay()),
			container.Inject<QuestProgressionDisplay>(new QuestProgressionDisplay()),
			container.Build<ReconnectPresenter>(),
			container.Build<TechPointsPresenter>(),
			container.Build<TierProgressionRewardPresenter>(),
			container.Build<ShowCampaignCompleteScreenEngine>(),
			container.Build<PurchaseConfirmedController>()
		});
		EditingInputPlugin editingInputPlugin = container.Inject<EditingInputPlugin>(new EditingInputPlugin());
		WorldSwitchInputPlugin worldSwitchInputPlugin = container.Inject<WorldSwitchInputPlugin>(new WorldSwitchInputPlugin());
		CharacterInputPlugin characterInputPlugin = container.Inject<CharacterInputPlugin>(new CharacterInputPlugin(cursorMode));
		InputEngine inputEngine = new InputEngine(container.Build<InputController>(), characterInputPlugin, worldSwitchInputPlugin, editingInputPlugin);
		_legacyEnginesRoot.AddEngine(inputEngine);
		_ticker.Add(inputEngine);
		_worldSwitching = container.Build<WorldSwitching>();
		_legacyEnginesRoot.AddEngine(new WorldSwitchingEngine(_worldSwitching));
		GUIShortcutTicker tickable = container.Inject<GUIShortcutTicker>(new GUIShortcutTicker(_guiInputController));
		_ticker.Add(tickable);
		_ticker.Add(container.Build<FriendInviteManager>());
		_ticker.Add(container.Build<ControlsDisplay>());
		_ticker.Add(container.Build<MachineEditorCollisionChecker>());
		_ticker.Add(container.Build<WeaponReorderDisplay>());
		_ticker.Add(container.Build<GarageSlotsPresenter>());
		_ticker.Add(container.Build<SlotOrderSaveController>());
		_ticker.Add(container.Build<IContextSensitiveXPRefresher>());
		WorldSwitchInputMothership component = new WorldSwitchInputMothership(_commandFactory, _guiInputController, container.Build<NormalBattleAvailability>(), container.Build<TeamDeathMatchAvailability>(), container.Build<DesiredGameMode>());
		_legacyEnginesRoot.AddComponent(component);
		_legacyEnginesRoot.AddComponent(container.Build<MirrorMode>());
		_legacyEnginesRoot.AddComponent(container.Build<AdvancedRobotEditSettings>());
		_legacyEnginesRoot.AddComponent(container.Build<RobotNudgeManager>());
		_legacyEnginesRoot.AddComponent(container.Build<PaintToolPresenter>());
		_legacyEnginesRoot.AddComponent(container.Build<CurrentToolMode>());
		_legacyEnginesRoot.AddComponent(container.Build<CentreRobot>());
		_legacyEnginesRoot.AddComponent(container.Build<BuildHistoryManager>());
		_legacyEnginesRoot.AddComponent(container.Build<BuildModeHUDVisibility>());
		BuildContainerInstances();
	}

	private void BuildContainerInstances()
	{
		container.Build<MachineColorUpdater>();
		container.Build<MachineUpdater>();
		BuildContextSpecificItems();
		container.Build<ClanComponentFactory>();
		container.Build<FriendComponentFactory>();
		_standardChatCommands = container.Inject<StandardChatCommands>(new StandardChatCommands());
		_contextNotifier.AddFrameworkDestructionListener(_standardChatCommands);
		_maintenanceModeController = container.Inject<MaintenanceModeController>(new MaintenanceModeController());
		_contextNotifier.AddFrameworkInitializationListener(_maintenanceModeController);
		_contextNotifier.AddFrameworkDestructionListener(_maintenanceModeController);
		_accountSanctions = container.Build<AccountSanctionsMothership>();
		_contextNotifier.AddFrameworkDestructionListener(_accountSanctions);
		_socialEventFeed = container.Inject<SocialEventFeed>(new SocialEventFeed());
		_chatCommandsMothership = container.Inject<ChatCommandsMothership>(new ChatCommandsMothership());
		_ticker.Add(container.Build<TestConnection>());
		_worldSwitchingAnalytics = container.Inject<WorldSwitchingMothershipAnalytics>(new WorldSwitchingMothershipAnalytics());
		_contextNotifier.AddFrameworkDestructionListener(_worldSwitchingAnalytics);
		_achievementsMasteryManager = container.Inject<AchievementCRFMasteryTracker>(new AchievementCRFMasteryTracker());
		_garageBaySwitcher = container.Inject<GarageBaySkinSwitcher>(new GarageBaySkinSwitcher());
		AudioLanguageSetter audioLanguageSetter = container.Inject<AudioLanguageSetter>(new AudioLanguageSetter());
		_contextNotifier.AddFrameworkDestructionListener(audioLanguageSetter);
		_robotShopClicksTracker = container.Inject<TLOG_RobotShopClicksTracker_Tencent>(new TLOG_RobotShopClicksTracker_Tencent());
	}

	public string GetTimestamp(DateTime value)
	{
		return value.ToString("yyyyMMddHHmmssffff");
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

	void IEntitySystemContext.AddComponent(IComponent component)
	{
		container.Inject<IComponent>(component);
		_legacyEnginesRoot.AddComponent(component);
	}

	void IEntitySystemContext.RemoveComponent(IComponent component)
	{
		_legacyEnginesRoot.RemoveComponent(component);
	}

	void ICompositionRoot.OnContextInitialized()
	{
		Console.Log("Back to mothership");
		ErrorWindow.Init(_gameObjectFactory);
		BuildGUI();
		_contextNotifier.NotifyFrameworkInitialized();
		WorldSwitching worldSwitching = (WorldSwitching)container.Build<IDispatchWorldSwitching>();
		worldSwitching.StartMothershipWithLastUsedMode();
	}

	private void BuildGUI()
	{
		container.Build<YourClanLayoutFactory>();
		container.Build<ClanInvitesLayoutFactory>();
		SocialGUIFactory socialGUIFactory = container.Build<SocialGUIFactory>();
		CustomGamePartyGUIFactory customGamePartyGUIFactory = container.Build<CustomGamePartyGUIFactory>();
		InvitePlayerToPartyGuiFactory invitePlayerToPartyGuiFactory = container.Build<InvitePlayerToPartyGuiFactory>();
		PopupContextMenuFactory popupContextMenuFactory = container.Build<PopupContextMenuFactory>();
		PlayerLevelInfoLayoutFactory playerLevelInfoLayoutFactory = container.Build<PlayerLevelInfoLayoutFactory>();
		DragAndDropGUIFactory dragAndDropGUIFactory = container.Build<DragAndDropGUIFactory>();
		TopBarDisplayFactory topBarDisplayFactory = container.Build<TopBarDisplayFactory>();
		RobotShopFactory robotShopFactory = container.Build<RobotShopFactory>();
		RobotShopTransactionFactory robotShopTransactionFactory = container.Build<RobotShopTransactionFactory>();
		ITopBarDisplay topBarDisplay = BuildContextSpecificTopBar();
		GameObject gameObject = _gameObjectFactory.Build("ChatPanel").get_transform().GetChild(0)
			.get_gameObject();
		socialGUIFactory.Build(gameObject, container);
		GameObject rootGUINode = _gameObjectFactory.Build("DragAndDropView");
		dragAndDropGUIFactory.Build(rootGUINode, container);
		GameObject gameObject2 = _gameObjectFactory.Build("CustomGamesParty").get_transform().GetChild(0)
			.get_gameObject();
		customGamePartyGUIFactory.Build(gameObject2, container);
		GameObject guiElementRoot = _gameObjectFactory.Build("InviteToParty_DropdownMenu");
		invitePlayerToPartyGuiFactory.Build(guiElementRoot, container);
		GameObject val = _gameObjectFactory.Build("InviteToParty_DropdownMenu");
		val.set_name("Custom Game Invite Dialog");
		val.get_transform().set_parent(gameObject2.GetComponentInChildren<CustomGamePartyGUIView>().get_transform());
		invitePlayerToPartyGuiFactory.Build(val, container);
		GameObject val2 = _gameObjectFactory.Build("GenericPopupMenuView");
		val2.GetComponent<GenericPopupMenuView>().popupMenu = PopupMenuType.Party;
		popupContextMenuFactory.Build(val2, container);
		topBarDisplayFactory.Build(topBarDisplay);
		robotShopFactory.Build();
		robotShopTransactionFactory.Build();
		playerLevelInfoLayoutFactory.Build(topBarDisplay, container, new PlayerLevelNeedRefreshObserver(container.Build<PlayerLevelNeedRefreshObservable>()));
		IEntityFactory val3 = _nodeEnginesRoot.GenerateEntityFactory();
		GameObject val4 = BuildGUIEntity(val3, "TechTree_Mockup");
		_peersLinkers.Introduce(val4.GetComponent<TechTreeViewImplementor>());
		GameObject val5 = _gameObjectFactory.Build("HintStatsPopup");
		StatsHintPopup component = val5.GetComponent<StatsHintPopup>();
		int instanceID = val5.GetInstanceID();
		val3.BuildEntity<StatsHintPopupEntityDescriptor>(instanceID, new object[1]
		{
			component
		});
		IGUIInputController iGUIInputController = container.Build<IGUIInputController>();
		TaskRunner.get_Instance().Run(TierProgressionScreenFactory.Build(_gameObjectFactory, container.Build<IServiceRequestFactory>(), container.Build<LoadingIconPresenter>(), iGUIInputController));
		_gameObjectFactory.Build("AwardedItemsScreen");
		iGUIInputController.AddDisplayScreens(new IGUIDisplay[1]
		{
			container.Build<AwardedItemsController>()
		});
		GameObject val6 = _gameObjectFactory.Build("GUI_OpsRoom");
		int instanceID2 = val6.GetInstanceID();
		MonoBehaviour[] componentsInChildren = val6.GetComponentsInChildren<MonoBehaviour>(true);
		List<object> list = new List<object>(componentsInChildren);
		list.Add(new OpsRoomCTAValuesImplementor(instanceID2));
		List<object> list2 = list;
		val3.BuildEntity<OpsRoomDisplayEntityDescriptor>(instanceID2, list2.ToArray());
		_robopassScreenFactory.Build(iGUIInputController);
		_robopassBSScreenFactory.BuildBattleSummaryUI(iGUIInputController);
		ItemShopDisplayFactory.Build(val3, _gameObjectFactory, iGUIInputController);
		ItemShopCTAFactory.BuildInTopBar(val3, topBarDisplay.GetTopBar().get_gameObject());
		IBrowser browser = new DummyBrowser();
		_priceChangeDialogFactory = new PriceChangeDialogFactory(_gameObjectFactory);
		_priceChangeDialogFactory.Build(_priceChangeDialogPresenter);
		container.Inject<IBrowser>(browser);
		BrowserURLChangedObserver browserURLChangedObserver = new BrowserURLChangedObserver(container.Build<BrowserURLChangedObservable>());
		BrowserClosedObserver browserClosedObserver = new BrowserClosedObserver(container.Build<BrowserClosedObservable>());
		ExecutePurchaseEngine executePurchaseEngine = new ExecutePurchaseEngine(container.Build<LoadingIconPresenter>(), _guiInputController, browser, browserURLChangedObserver, browserClosedObserver, container.Build<PurchaseRefresher>(), container.Build<IAnalyticsRequestFactory>(), _nodeEnginesRoot.GenerateEntityFunctions(), _priceChangeDialogPresenter);
		_nodeEnginesRoot.AddEngine(executePurchaseEngine);
	}

	private GameObject BuildGUIEntity(IEntityFactory entityFactory, string prefabName)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		GameObject gameObject = _gameObjectFactory.Build(prefabName).get_gameObject();
		IInitializableImplementor[] componentsInChildren = gameObject.GetComponentsInChildren<IInitializableImplementor>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Initialize();
		}
		IEntityDescriptorHolder[] componentsInChildren2 = gameObject.GetComponentsInChildren<IEntityDescriptorHolder>(true);
		foreach (IEntityDescriptorHolder val in componentsInChildren2)
		{
			MonoBehaviour val2 = val;
			GameObject gameObject2 = val2.get_gameObject();
			MonoBehaviour[] components = gameObject2.GetComponents<MonoBehaviour>();
			entityFactory.BuildEntity(gameObject2.GetInstanceID(), val.RetrieveDescriptor(), (object[])components);
		}
		return gameObject;
	}

	void ICompositionRoot.OnContextDestroyed()
	{
		_contextNotifier.NotifyFrameworkDeinitialized();
		ErrorWindow.TearDown();
	}

	private static void ClearUpServiceLayer()
	{
		PhotonWebServicesUtility.TearDown();
		PhotonChatUtility.TearDown();
		PhotonSocialUtility.TearDown();
		PhotonSinglePlayerUtility.TearDown();
		PhotonLobbyUtility.TearDown();
	}
}
