using Svelto.Factories;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

internal class LoadingIconPresenter
{
	public const string Autosaving = "Autosaving";

	public const string PropPresenter = "PropPresenter";

	public const string RoboshopLoadingIcon = "RobotShopLoadingScreen";

	public const string RoboshopMachineBuild = "RobotShopMachineBuild";

	public const string LoadingSinglePlayerCampaigns = "LoadingSinglePlayerCampaigns";

	public const string LoadingSinglePlayerCampaignOverrides = "LoadingSinglePlayerCampaignOverrides";

	public const string TechPointsTracker = "TechPointsTrackerService";

	public const string WalletTracker = "WalletTrackerService";

	public const string GarageLoading = "GarageLoadingIcon";

	public const string GarageSlotEdit = "GarageSlotLoadingIcon";

	public const string GarageSlotPurchase = "GarageSlotPurchaseLoadingIcon";

	public const string GarageRefresh = "GarageRefreshLoadingIcon";

	public const string CPULoadOnSinglePlayer = "GetCPULoadOnSinglePlayer";

	public const string AvatarInfoRequest = "AvatarInfoRequest";

	public const string LoadWeaponOrder = "LoadWeaponOrder";

	public const string ConfirmRating = "ConfirmRating";

	public const string LoadCurrentLeagueAndStarsInCubeForge = "CubeForgeLoadCurrentLeague";

	public const string LoadBestLeagueAndStarsInCubeForge = "CubeForgeLoadBestLeague";

	public const string LoadingItemShop = "LoadingItemShop";

	public const string RobotConfiguration = "RobotConfiguration";

	public const string PurchaseItemShop = "PurchaseItemShop";

	public const string UpdateShotRobotOffset = "UpdateShotRobot";

	public const string WaitForFlag = "WaitForFlag";

	public const string ReturningToMothershipFromTutorial = "ReturningToMothershipFromTutorial";

	public const string StartTutorial = "startTutorial";

	public const string InitialMothershipGUIFlow = "InitialMothershipGUIFlow";

	public const string LoadTutorialGUI = "TutorialGUILoading";

	public const string TutorialSimulationFlow = "TutorialSimulationFlow";

	public const string ClanSection = "Clans";

	public const string FriendSection = "Friends";

	public const string PartyTierCheck = "PartyTierCheck";

	public const string BattleExperienceSection = "BattleExperience";

	public const string CampaignRobotValidation = "CampaignRobotValidation";

	public const string PurchaseScreen = "PurchaseScreen";

	public const string CheckPendingInvitation = "CheckPendingInvitation";

	public const string AvatarSelection = "AvatarSelection";

	public const string QueryPlatoonStatus = "QueryPlatoonStatus";

	public const string GetPlatoonData = "GetPlatoonData";

	public const string Checkingpartystatus = "Checkingpartystatus";

	public const string AcceptInvitation = "AcceptInvitation";

	public const string DeclineInvitation = "DeclineInvitation";

	public const string RemoveFromParty = "RemoveFromParty";

	public const string LeaveParty = "LeaveParty";

	public const string XSollaShop = "XSollaShop";

	public const string LeagueScreen = "LeagueScreen";

	public const string LeagueRewardsScreen = "LeagueRewardsScreen";

	public const string ValidateSeasonReward = "ValidateSeasonReward";

	public const string LoadClanData = "LoadClanData";

	public const string BrawlParameters = "BrawlParameters";

	public const string BrawlOverrides = "BrawlOverrides";

	public const string BrawlLanguageStrings = "BrawlLanguageStrings";

	public const string CustomGameOverrides = "CustomGameOverrides";

	public const string PlayerLevelInfo = "PlayerLevelInfo";

	public const string EnterPlanetDialog = "EnterPlanetDialog";

	public const string LobbyLoadingScreen = "LobbyLoadingScreen";

	public const string CustomGameScreen = "CustomGameScreen";

	public const string LeaveCustomGame = "LeaveCustomGame";

	public const string CreateCustomGame = "CreateCustomGame";

	public const string CustomGameConfig = "CustomGameConfig";

	public const string RetrieveCustomGame = "RetrieveCustomGame";

	public const string CustomGameTeamController = "CustomGameTeamController";

	public const string DispatchCustomGameInvite = "DispatchCustomGameInvite";

	public const string RespondToCustomGameInvitation = "RespondToCustomGameInvitation";

	public const string KickCancelFromCustomGame = "KickOrCancelFromCustomGame";

	public const string SwapCustomGame = "SwapCustomGame";

	public const string CheckCanBeInvited = "CheckCanBeInvited";

	public const string CheckWasInvited = "CheckWasInvited";

	public const string CheckCanQueue = "CheckCanQueue";

	public const string CheckIsInMultiplayerParty = "CheckIsInMultiplayerParty";

	public const string ForceLeaveMultiplayerParty = "ForceLeaveMultiplayerParty";

	public const string LoadPowerBarSettings = "LoadPowerBarSettings";

	public const string ConstructingRobot = "ConstructingRobot";

	public const string DamageBoostSettings = "DamageBoostSettings";

	public const string TauntsData = "TauntsData";

	public const string LoadingTutorial = "Loadingtutorial";

	public const string CreatingPrebuiltRobot = "CreatingPrebuiltRobot";

	public const string MaxGarageSlots = "MaxGarageSlots";

	public const string RobotBuilding = "RobotBuilding";

	public const string LoadingPlatformConfiguration = "LoadingPlatformConfiguration";

	public const string LoadingTiersData = "LoadingTiersData";

	public const string LoadingCPUSettings = "LoadingCPUSettings";

	public const string UnlockCubeType = "UnlockCubeType";

	public const string SplashScreenLoading = "LoadingSplashScreenData";

	public const string CosmeticCreditsService = "CosmeticCreditsService";

	public const string CubeInventory = "CubeInventory";

	public const string SaveVotesAfterBattle = "SaveVotesAfterBattle";

	public const string LoadVotingAfterBattleThresholds = "LoadVotingAfterBattleThresholds";

	public const string RefreshAccountSanctionsData = "RefreshAccountSanctionsData";

	public const string LoadCPUPowerData = "LoadCPUPowerData";

	public const string GetReconnectableGame = "GetReconnectableGame";

	public const string DeclineReconnect = "UnregisterFromReconnectableGame";

	public const string LoadingDailyQuests = "LoadingDailyQuests";

	public const string ReplaceDailyQuests = "ReplaceDailyQuests";

	public const string MarkNotifiedCompletedDailyQuests = "MarkNotifiedCompletedDailyQuests";

	public const string MarkQuestsAsSeen = "MarkQuestsAsSeen";

	public const string UpdatingDailyQuestsProgress = "UpdatingDailyQuestsProgress";

	public const string TechTreeLoading = "TechTreeLoading";

	public const string TechPointsAwardLoading = "TechPointsAwardLoading";

	public const string CheckingRobotSanction = "CheckingRobotSanction";

	public const string LoadingBaySkinConfiguration = "LoadingBaySkinConfiguration";

	public const string GetPlayerBaySkinList = "GetPlayerBaySkinList";

	public const string GetRobotBaySkinID = "GetRobotBaySkinID";

	public const string LoadGarageBaySkin = "LoadGarageBaySkin";

	public const string LoadGameModePreferences = "LoadingGameModePreferences";

	public const string SaveGameModePreferences = "SavingGameModePreferences";

	public const string LoadCurrentLeagueAndStarsInPlayScreen = "LoadCurrentLeagueAndStarsInPlayScreen";

	public const string GetLastCompletedCampaign = "GetLastCompletedCampaign";

	public const string MarkLastCompletedCampaignAsSeen = "MarkLastCompletedCampaignAsSeen";

	public const string ApplyingPromoCode = "ApplyingPromoCode";

	public const string CheckingCubeRewards = "CheckingCubeRewards";

	public const string HandleAnalytics = "HandleAnalytics";

	public const string RealMoneyStore = "RealMoneyStore";

	public const string RealMoneyStorePossibleRoboPassItems = "RealMoneyStorePossibleRoboPassItems";

	public const string BuyPremiumAfterBattle = "BuyPremiumAfterBattle";

	public const string BuyRoboPassAfterBattle = "BuyRoboPassAfterBattle";

	private readonly Stack<float> _stack = new Stack<float>();

	private GameObject _loadingIcon;

	private Animation _animation;

	private AnimatedAlpha _widgetAlpha;

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		private get;
		set;
	}

	public bool forceOpaque
	{
		private get;
		set;
	}

	public bool isLoading
	{
		get;
		private set;
	}

	public GenericLoadingScreen loadingScreen
	{
		get;
		private set;
	}

	public void NotifyLoading(string name, bool opaque = false)
	{
		NotifyLoading(name, StringTableBase<StringTable>.Instance.GetString("strContactingServer"), opaque);
	}

	public void NotifyLoading(string name, string iconName, bool opaque = false)
	{
		if (_loadingIcon == null)
		{
			_loadingIcon = gameObjectFactory.Build("GenericLoadingDialog");
			loadingScreen = _loadingIcon.GetComponent<GenericLoadingScreen>();
			_animation = _loadingIcon.GetComponent<Animation>();
			_widgetAlpha = _loadingIcon.GetComponent<AnimatedAlpha>();
		}
		_loadingIcon.set_name(name);
		loadingScreen.text = iconName;
		if (_stack.Count == 0)
		{
			_loadingIcon.SetActive(true);
			isLoading = true;
		}
		_stack.Push(loadingScreen.backgroundOpacity);
		if (opaque || forceOpaque)
		{
			_widgetAlpha.alpha = 1f;
			loadingScreen.backgroundOpacity = 1f;
		}
		else if (_animation != null)
		{
			_widgetAlpha.alpha = 0f;
			_animation.Play();
		}
	}

	public void NotifyLoadingDone(string name)
	{
		if (_stack.Count > 0)
		{
			float backgroundOpacity = _stack.Pop();
			if (!forceOpaque)
			{
				loadingScreen.backgroundOpacity = backgroundOpacity;
			}
			if (_stack.Count == 0)
			{
				_loadingIcon.SetActive(false);
				isLoading = false;
			}
		}
	}

	public void ChangeLoadingIconText(string text)
	{
		loadingScreen.text = text;
	}
}
