using Svelto.DataStructures;
using System.Collections.Generic;

internal static class AnalyticsEvent
{
	public static void Initialise(string currentBuild, string userName, bool isNewUser)
	{
		DeltaDNAHelper.InitialiseDeltaDNA(currentBuild, userName, isNewUser);
	}

	public static void BI_FriendAcceptInvite()
	{
		DeltaDNAHelper.SendEvent(DeltaDNAEventName.friendInviteAccepted);
	}

	public static void SendFrameRate(string gameContext, int avgFPS, int sdFPS)
	{
		DeltaDNAHelper.SendFrameRate(gameContext, avgFPS, sdFPS);
	}

	public static void SendError(string error)
	{
		DeltaDNAHelper.SendError(error);
	}

	public static void LogClaimedSteamPromotion(string bundleName, float priceUSD)
	{
		DeltaDNAHelper.SendSteamBundlePurchased(bundleName, priceUSD);
	}

	public static void SendGameStartedEvent(AnalyticsLaunchMode launchMode, AnalyticsLoginType loginType, bool emailValidated)
	{
		DeltaDNAHelper.SendGameStartedEvent(launchMode.ToString(), loginType.ToString(), emailValidated);
	}

	public static void SendLoadingEvent(string operation)
	{
		DeltaDNAHelper.SendGameLoadingEvent(operation);
	}

	public static void SendOnLoggedInPlayerData(int playerLevel, uint playerXP, long robitsBalance, long ccBalance, bool isDeveloper, int totalFriends, string clanName, AnalyticsPremiumSubscriptionType premiumType, int techsUnlocked, int? roboPassXP, bool? roboPassPlus, string abTest, string abTestGroupName)
	{
		DeltaDNAHelper.SendOnLoginPlayerData(playerLevel, playerXP, robitsBalance, ccBalance, isDeveloper, totalFriends, clanName, premiumType.ToString(), techsUnlocked, roboPassXP, roboPassPlus, abTest, abTestGroupName);
	}

	public static void SendOnLoggedInSettings(string resolution, string gfx, bool fullScreen, bool capFrameRateEnabled, int capFrameRateAmount, bool zoomMode, bool invertY, bool showCenterOfMass, bool blockFriendClanInvites, bool acceptFriendClanOnlyInvites, string language, string processorType, int memorySize, int shaderLevel)
	{
		DeltaDNAHelper.SendOnLoggedInSettings(resolution, gfx, fullScreen, capFrameRateEnabled, capFrameRateAmount, zoomMode, invertY, showCenterOfMass, blockFriendClanInvites, acceptFriendClanOnlyInvites, language, processorType, memorySize, shaderLevel);
	}

	public static void SendEvent(DeltaDNAEventName eventName)
	{
		DeltaDNAHelper.SendEvent(eventName);
	}

	public static void SendLevelStarted(string battleID, string levelName, string gameModeType)
	{
		DeltaDNAHelper.SendLevelStarted(battleID, levelName, gameModeType);
	}

	public static void SendLevelStarted(string battleID, string levelName, string gameModeType, uint robotCPU, uint? robotTier, bool isCRFBot, string controlType, bool verticalStrafing, int totalCosmetics, uint? aiPlayers)
	{
		DeltaDNAHelper.SendLevelStarted(battleID, levelName, gameModeType, robotCPU, robotTier, isCRFBot, controlType, verticalStrafing, totalCosmetics, aiPlayers);
	}

	public static void SendLevelEnded(string levelName, string gameModeType, int duration)
	{
		DeltaDNAHelper.SendLevelEnded(levelName, gameModeType, duration);
	}

	public static void SendLevelEnded(string levelName, string gameModeType, string battleResult, uint totalPlayerScore, int duration)
	{
		DeltaDNAHelper.SendLevelEnded(levelName, gameModeType, battleResult, totalPlayerScore, duration);
	}

	public static void SendBattleWon(string levelName, string gameModeType, string battleResult, Dictionary<string, uint> playerScores)
	{
		DeltaDNAHelper.SendBattleResult(DeltaDNAEventName.battleWon.ToString(), battleResult, levelName, gameModeType, playerScores);
	}

	public static void SendBattleLost(string levelName, string gameModeType, string battleResult, Dictionary<string, uint> playerScores)
	{
		DeltaDNAHelper.SendBattleResult(DeltaDNAEventName.battleLost.ToString(), battleResult, levelName, gameModeType, playerScores);
	}

	public static void SendBattleDraw(string levelName, string gameModeType, string battleResult, Dictionary<string, uint> playerScores)
	{
		DeltaDNAHelper.SendBattleResult(DeltaDNAEventName.battleDraw.ToString(), battleResult, levelName, gameModeType, playerScores);
	}

	public static void SendBattleQuit(string levelName, string gameModeType)
	{
		DeltaDNAHelper.SendLeftBattleBeforeEnd(DeltaDNAEventName.battleQuit.ToString(), levelName, gameModeType);
	}

	public static void SendBattleLeft(string levelName, string gameModeType)
	{
		DeltaDNAHelper.SendLeftBattleBeforeEnd(DeltaDNAEventName.battleLeft.ToString(), levelName, gameModeType);
	}

	public static void SendDisconnectedFromGameServer(string levelName, string gameModeType, string errorCode)
	{
		DeltaDNAHelper.SendDisconnected(DeltaDNAEventName.battleDisconnectByGameServer, levelName, gameModeType, errorCode);
	}

	public static void SendDisconnectedByClient(string levelName, string gameModeType)
	{
		DeltaDNAHelper.SendDisconnected(DeltaDNAEventName.battleDisconnectByClient, levelName, gameModeType, null);
	}

	public static void SendClanEvent(DeltaDNAEventName eventName, string clanName)
	{
		DeltaDNAHelper.SendClanEvent(eventName.ToString(), clanName);
	}

	public static void SendCRFCollectedEarnings(int totalRobits, int earnings)
	{
		DeltaDNAHelper.SendCRFCollectedEarnings(totalRobits, earnings);
	}

	public static void SendRobotDownloaded(uint tier, uint cpu, FasterList<ItemCategory> movements, FasterList<ItemCategory> weapons)
	{
		DeltaDNAHelper.SendRobotDownloaded(tier, cpu, movements, weapons);
	}

	public static void SendRobotUploaded(uint tier, uint cpu, FasterList<ItemCategory> movements, FasterList<ItemCategory> weapons)
	{
		DeltaDNAHelper.SendRobotUploaded(tier, cpu, movements, weapons);
	}

	public static void SendRobotControlsChanged(string controlSetting, bool verticalStrafing, bool sidewaysDriving, bool tracksTurnOnSpot)
	{
		DeltaDNAHelper.SendRobotControlsChanged(controlSetting, verticalStrafing, sidewaysDriving, tracksTurnOnSpot);
	}

	public static void SendChatSent(ChatChannelType channel, int totalCharacters)
	{
		DeltaDNAHelper.SendChatSent(channel.ToString(), totalCharacters);
	}

	public static void SendChatJoined(ChatChannelType channel)
	{
		DeltaDNAHelper.SendChatJoined(channel.ToString());
	}

	public static void SendSettingsChanged(float? musicVolume, float? sfxVolume, float? speechVolume, float? buildMouseSpeed, float? fightMouseSpeed, string language, bool? buildHintsEnabled)
	{
		DeltaDNAHelper.SendSettingsChanged(musicVolume, sfxVolume, speechVolume, buildMouseSpeed, fightMouseSpeed, language, buildHintsEnabled);
	}

	public static void SendCollectedSeasonReward(int robits)
	{
		DeltaDNAHelper.SendCollectedSeasonReward(robits);
	}

	public static void SendLevelUp(int level, string abTest, string abTestGroup)
	{
		DeltaDNAHelper.SendLevelUp(level, abTest, abTestGroup);
	}

	public static void SendPromoCodeActivated(string promoId, float priceUSD, string bundleId)
	{
		DeltaDNAHelper.SendPromoCodeActivated(promoId, priceUSD, bundleId);
	}

	public static void SendAsksToReconnect()
	{
		DeltaDNAHelper.SendAskedToReconnect();
	}

	public static void SendReconnected(float timeTaken)
	{
		DeltaDNAHelper.SendReconnected(timeTaken);
	}

	public static void SendCubesUnlocked(uint techPointsCost, string cubeNameKey, int techsUnlocked)
	{
		DeltaDNAHelper.SendCubesUnlocked(techPointsCost, cubeNameKey, techsUnlocked);
	}

	public static void SendCampaignWaveSummary(string campaignNameKey, int campaignDifficulty, int waveNumber, int waveDuration, int livesLost, string result, int kills)
	{
		DeltaDNAHelper.SendCampaignWaveSummary(campaignNameKey, campaignDifficulty, waveNumber, waveDuration, livesLost, result, kills);
	}

	public static void SendTierRankUp(uint tier, string rank)
	{
		DeltaDNAHelper.SendTierRankUp(tier, rank);
	}

	public static void SendItemBought(string item, string itemType, string currency, int cost, string context, int discount)
	{
		DeltaDNAHelper.SendItemBought(item, itemType, currency, cost, context, discount);
	}

	public static void SendItemStocked(string item, string itemType, string currency, string context, string restock, bool locked, int discount)
	{
		DeltaDNAHelper.SendItemStocked(item, itemType, currency, context, restock, locked, discount);
	}

	public static void SendItemShopVisited(string context)
	{
		DeltaDNAHelper.SendItemShopVisited(context);
	}

	public static void SendXpEarned(int earned, int userXP, int? roboPassXP, int userLevel, int premiumBonus, string source, string sourceDetail)
	{
		DeltaDNAHelper.SendXpEarned(earned, userXP, roboPassXP, userLevel, premiumBonus, source, sourceDetail);
	}

	public static void SendCurrencyEarned(string currency, int earned, long balance, int premiumBonus, string source, string sourceDetail)
	{
		DeltaDNAHelper.SendCurrencyEarned(currency, earned, balance, premiumBonus, source, sourceDetail);
	}

	public static void SendCurrencySpent(string currency, int spent, long balance, string sink, string sinkDetail)
	{
		DeltaDNAHelper.SendCurrencySpent(currency, spent, balance, sink, sinkDetail);
	}

	public static void SendQuestAdded(string questID, int activeQuests)
	{
		DeltaDNAHelper.SendQuestAdded(questID, activeQuests);
	}

	public static void SendQuestCompleted(string questID, int activeQuests)
	{
		DeltaDNAHelper.SendQuestCompleted(questID, activeQuests);
	}

	public static void SendQuestRerolled(string questID, int activeQuests)
	{
		DeltaDNAHelper.SendQuestRerolled(questID, activeQuests);
	}

	public static void SendPurchaseFunnelStart(string step, string context, string eventId)
	{
		DeltaDNAHelper.SendPurchaseFunnelStart(step, context, eventId);
	}

	public static void SendPurchaseFunnelStep(string step, string context, string startEventId)
	{
		DeltaDNAHelper.SendPurchaseFunnelStep(step, context, startEventId);
	}

	public static void SendRoboPassGradeUp(string season, int grade)
	{
		DeltaDNAHelper.SendRoboPassGradeUp(season, grade);
	}

	public static void SendRoboPassRewardCollected(string prize, int amount, string season, bool roboPassPlus, int grade)
	{
		DeltaDNAHelper.SendRoboPassRewardCollected(prize, amount, season, roboPassPlus, grade);
	}
}
