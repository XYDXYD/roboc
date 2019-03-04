using DeltaDNA;
using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

internal class DeltaDNAHelper : MonoBehaviour
{
	private static bool isRunning;

	private const string LIVE_ENVIRONMENT_KEY = "64541189536650470521242598714902";

	private const string DEV_ENVIRONMENT_KEY = "64541183263581421708211682914902";

	private const string COLLECT_URL = "https://collect11163rbcrf.deltadna.net/collect/api";

	private const string ENGAGE_URL = "https://engage11163rbcrf.deltadna.net";

	static DeltaDNAHelper()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		GameObject val = new GameObject("DeltaDNAListener");
		val.AddComponent<DNABehaviour>();
		Object.DontDestroyOnLoad(val);
	}

	public DeltaDNAHelper()
		: this()
	{
	}

	public static void InitialiseDeltaDNA(string buildNo, string username, bool isNewUser)
	{
		string text = "64541183263581421708211682914902";
		if (!ServEnv.Exists())
		{
			text = "64541189536650470521242598714902";
		}
		Singleton<DDNA>.get_Instance().get_Settings().set_OnInitSendGameStartedEvent(false);
		SetIsNewToDeltaDNA(username, isNewUser);
		isRunning = true;
		Singleton<DDNA>.get_Instance().set_ClientVersion(buildNo);
		Singleton<DDNA>.get_Instance().StartSDK(text, "https://collect11163rbcrf.deltadna.net/collect/api", "https://engage11163rbcrf.deltadna.net", username);
	}

	public static void SendGameStartedEvent(string platform, string loginType, bool emailValidated)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.gameStarted.ToString());
			val.AddParam("gamingPlatform", (object)platform);
			val.AddParam("loginType", (object)loginType);
			val.AddParam("emailValidated", (object)emailValidated);
			val.AddParam("clientVersion", (object)Singleton<DDNA>.get_Instance().get_ClientVersion());
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendOnLoginPlayerData(int playerLevel, uint playerXP, long robitsBalance, long ccBalance, bool isDeveloper, int totalFriends, string clanName, string premiumType, int techsUnlocked, int? roboPassXP, bool? roboPassPlus, string abTest, string abTestGroupName)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.loggedin.ToString());
			if (abTest != null && abTestGroupName != null)
			{
				val.AddParam("abGroupName", (object)(abTest + " " + abTestGroupName));
			}
			val.AddParam("userLevel", (object)playerLevel);
			val.AddParam("userXP", (object)playerXP);
			val.AddParam("robits", (object)robitsBalance);
			val.AddParam("cc", (object)ccBalance);
			val.AddParam("isDeveloper", (object)isDeveloper);
			val.AddParam("totalFriends", (object)totalFriends);
			if (!string.IsNullOrEmpty(clanName))
			{
				val.AddParam("clanName", (object)clanName);
			}
			val.AddParam("premiumSubscriptionType", (object)premiumType);
			val.AddParam("techsUnlocked", (object)techsUnlocked);
			if (roboPassXP.HasValue && roboPassPlus.HasValue)
			{
				val.AddParam("roboPassXP", (object)roboPassXP.Value);
				val.AddParam("roboPassPlus", (object)roboPassPlus.Value);
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendOnLoggedInSettings(string resolution, string gfx, bool fullScreen, bool capFrameRateEnabled, int capFrameRateAmount, bool zoomMode, bool invertY, bool showCenterOfMass, bool blockFriendClanInvites, bool acceptFriendClanOnlyInvites, string language, string processorType, int memorySize, int shaderLevel)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.loggedinsettings.ToString());
			val.AddParam("resolution", (object)resolution);
			val.AddParam("gfx", (object)gfx);
			val.AddParam("fullscreen", (object)fullScreen);
			val.AddParam("capFrameRateEnabled", (object)capFrameRateEnabled);
			val.AddParam("capFrameRateAmount", (object)capFrameRateAmount);
			val.AddParam("zoomModeEnabled", (object)zoomMode);
			val.AddParam("invertYEnabled", (object)invertY);
			val.AddParam("showCenterOfMassEnabled", (object)showCenterOfMass);
			val.AddParam("blockFriendClanInvitesEnabled", (object)blockFriendClanInvites);
			val.AddParam("acceptOnlyFriendClanInvitesEnabled", (object)acceptFriendClanOnlyInvites);
			val.AddParam("language", (object)language);
			val.AddParam("processorType", (object)processorType);
			val.AddParam("memorySize", (object)memorySize);
			val.AddParam("shaderLevel", (object)shaderLevel);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendLevelStarted(string battleID, string levelName, string gameMode)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.levelStarted.ToString());
			if (!string.IsNullOrEmpty(battleID))
			{
				val.AddParam("battleID", (object)battleID);
			}
			val.AddParam("levelName", (object)levelName);
			if (!string.IsNullOrEmpty(gameMode))
			{
				val.AddParam("gameMode", (object)gameMode);
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendLevelStarted(string battleID, string levelName, string gameMode, uint robotCPU, uint? robotTier, bool isCRFBot, string controlType, bool verticalStrafing, int totalCosmetics, uint? aiPlayers)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.levelStarted.ToString());
			if (!string.IsNullOrEmpty(battleID))
			{
				val.AddParam("battleID", (object)battleID);
			}
			val.AddParam("levelName", (object)levelName);
			val.AddParam("cpu", (object)robotCPU);
			if (robotTier.HasValue)
			{
				val.AddParam("tier", (object)robotTier.Value);
			}
			if (aiPlayers.HasValue)
			{
				val.AddParam("aiPlayers", (object)aiPlayers.Value);
			}
			val.AddParam("isCRFRobot", (object)isCRFBot);
			val.AddParam("verticalStrafing", (object)verticalStrafing);
			val.AddParam("totalCosmetics", (object)totalCosmetics);
			if (!string.IsNullOrEmpty(gameMode))
			{
				val.AddParam("gameMode", (object)gameMode);
			}
			if (!string.IsNullOrEmpty(controlType))
			{
				val.AddParam("controlType", (object)controlType);
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendLevelEnded(string levelName, string gameMode, int duration)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.levelEnded.ToString());
			val.AddParam("levelName", (object)levelName);
			if (!string.IsNullOrEmpty(gameMode))
			{
				val.AddParam("gameMode", (object)gameMode);
			}
			val.AddParam("duration", (object)duration);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendLevelEnded(string levelName, string gameMode, string battleResult, uint totalPlayerScore, int duration)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.levelEnded.ToString());
			val.AddParam("levelName", (object)levelName);
			val.AddParam("battleResult", (object)battleResult);
			if (totalPlayerScore != 0)
			{
				val.AddParam("userScore", (object)totalPlayerScore);
			}
			if (!string.IsNullOrEmpty(gameMode))
			{
				val.AddParam("gameMode", (object)gameMode);
			}
			val.AddParam("duration", (object)duration);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendBattleResult(string battleEventName, string battleResult, string levelName, string gameMode, Dictionary<string, uint> playerScores)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(battleEventName);
			val.AddParam("levelName", (object)levelName);
			val.AddParam("battleResult", (object)battleResult);
			if (!string.IsNullOrEmpty(gameMode))
			{
				val.AddParam("gameMode", (object)gameMode);
			}
			Dictionary<string, uint>.Enumerator enumerator = playerScores.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, uint> current = enumerator.Current;
				val.AddParam(current.Key, (object)current.Value);
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendLeftBattleBeforeEnd(string battleLeftEventName, string levelName, string gameMode)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(battleLeftEventName);
			val.AddParam("levelName", (object)levelName);
			val.AddParam("gameMode", (object)gameMode);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendDisconnected(DeltaDNAEventName eventName, string levelName, string gameMode, string errorCode)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(eventName.ToString());
			val.AddParam("levelName", (object)levelName);
			val.AddParam("gameMode", (object)gameMode);
			if (!string.IsNullOrEmpty(errorCode))
			{
				val.AddParam("gameServerErrorCode", (object)errorCode);
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendRobotDownloaded(uint tier, uint cpu, FasterList<ItemCategory> movements, FasterList<ItemCategory> weapons)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		if (isRunning)
		{
			string str = string.Empty;
			GameEvent val = new GameEvent(DeltaDNAEventName.robotDownloaded.ToString());
			val.AddParam("tier", (object)tier);
			val.AddParam("cpu", (object)cpu);
			for (int i = 0; i < weapons.get_Count(); i++)
			{
				ItemCategory itemCategory = weapons.get_Item(i);
				str = str + itemCategory.ToString() + " ";
				val.AddParam($"weapon{i}", (object)itemCategory.ToString());
			}
			for (int j = 0; j < movements.get_Count(); j++)
			{
				ItemCategory itemCategory2 = movements.get_Item(j);
				str = str + itemCategory2.ToString() + " ";
				val.AddParam($"movement{j}", (object)itemCategory2.ToString());
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendCRFCollectedEarnings(int totalRobots, int earnings)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.crfCollectedEarnings.ToString());
			val.AddParam("totalRobotsSold", (object)totalRobots);
			val.AddParam("robitsEarned", (object)earnings);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendEvent(DeltaDNAEventName eventName)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(eventName.ToString());
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendRobotUploaded(uint tier, uint robotCPU, FasterList<ItemCategory> movements, FasterList<ItemCategory> weapons)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.robotUploaded.ToString());
			val.AddParam("tier", (object)tier);
			val.AddParam("cpu", (object)robotCPU);
			for (int i = 0; i < weapons.get_Count(); i++)
			{
				ItemCategory itemCategory = weapons.get_Item(i);
				val.AddParam($"weapon{i}", (object)itemCategory.ToString());
			}
			for (int j = 0; j < movements.get_Count(); j++)
			{
				ItemCategory itemCategory2 = movements.get_Item(j);
				val.AddParam($"movement{j}", (object)itemCategory2.ToString());
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendRobotControlsChanged(string controlSetting, bool verticalStrafing, bool sidewaysDriving, bool tracksTurnOnSpot)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.robotControlsChanged.ToString());
			val.AddParam("controlType", (object)controlSetting);
			val.AddParam("verticalStrafing", (object)verticalStrafing);
			val.AddParam("sidewaysDriving", (object)sidewaysDriving);
			val.AddParam("tracksTurnOnSpot", (object)tracksTurnOnSpot);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendClanEvent(string eventName, string clanName)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(eventName);
			val.AddParam("clanName", (object)clanName);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendChatSent(string channel, int totalCharacters)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.chatSent.ToString());
			val.AddParam("chatChannel", (object)channel);
			val.AddParam("totalCharacters", (object)totalCharacters);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendChatJoined(string channel)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.chatJoined.ToString());
			val.AddParam("chatChannel", (object)channel);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendControlsChanged(int totalControls)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.controlsChanged.ToString());
			val.AddParam("totalControls", (object)totalControls);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendSettingsChanged(float? musicVolume, float? sfxVolume, float? speechVolume, float? buildMouseSpeed, float? fightMouseSpeed, string language, bool? buildHintsEnabled)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.settingsChanged.ToString());
			if (musicVolume.HasValue)
			{
				val.AddParam("musicVolume", (object)musicVolume.Value);
			}
			if (sfxVolume.HasValue)
			{
				val.AddParam("sfxVolume", (object)sfxVolume.Value);
			}
			if (speechVolume.HasValue)
			{
				val.AddParam("speechVolume", (object)speechVolume.Value);
			}
			if (buildMouseSpeed.HasValue)
			{
				val.AddParam("buildMouseSpeed", (object)buildMouseSpeed.Value);
			}
			if (fightMouseSpeed.HasValue)
			{
				val.AddParam("fightMouseSpeed", (object)fightMouseSpeed.Value);
			}
			if (language != null)
			{
				val.AddParam("language", (object)language);
			}
			if (buildHintsEnabled.HasValue)
			{
				val.AddParam("buildHintsEnabled", (object)buildHintsEnabled.Value);
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendCollectedSeasonReward(int robits)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.seasonRewardCollected.ToString());
			val.AddParam("robits", (object)robits);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendLevelUp(int level, string abTest, string abTestGroupName)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.levelUp.ToString());
			val.AddParam("userLevel", (object)level);
			val.AddParam("levelUpName", (object)"playerLevelUp");
			if (abTest != null && abTestGroupName != null)
			{
				val.AddParam("abGroupName", (object)(abTest + " " + abTestGroupName));
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendSteamBundlePurchased(string bundleName, float priceUSD)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_003e: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent("transaction").AddParam("productsSpent", (object)new Product().SetRealCurrency("USD", Product<Product>.ConvertCurrency("USD", (decimal)priceUSD)));
			val.AddParam("productsReceived", (object)new Product().AddItem(bundleName, "SteamBundle", 1));
			val.AddParam("transactionType", (object)"PURCHASE");
			val.AddParam("transactionName", (object)"Steam Purchase");
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendPromoCodeActivated(string promoId, float priceUSD, string bundleId)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0049: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		if (isRunning)
		{
			if (priceUSD > 0f)
			{
				GameEvent val = new GameEvent("transaction").AddParam("productsSpent", (object)new Product().SetRealCurrency("USD", Product<Product>.ConvertCurrency("USD", (decimal)priceUSD)));
				val.AddParam("productsReceived", (object)new Product().AddItem(promoId, "Promo", 1));
				val.AddParam("transactionType", (object)"PURCHASE");
				val.AddParam("transactionName", (object)"Promotion Purchase");
				Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
			}
			else
			{
				GameEvent val2 = new GameEvent(DeltaDNAEventName.promoCodeActivated.ToString());
				val2.AddParam("promoId", (object)promoId);
				val2.AddParam("bundleId", (object)bundleId);
				Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val2);
			}
		}
	}

	public static void SendFrameRate(string gameContext, int avgFPS, int sdFPS)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.framerate.ToString());
			val.AddParam("framerateAvg", (object)avgFPS);
			val.AddParam("framerateSD", (object)sdFPS);
			if (Singleton<DDNA>.get_Instance() != null)
			{
				Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
			}
		}
	}

	public static void SendError(string error)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.error.ToString());
			val.AddParam("error", (object)error);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendGameLoadingEvent(string operation)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.gameLoading.ToString());
			val.AddParam("loadingOperation", (object)operation);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendAskedToReconnect()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.askedToReconnect.ToString());
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendReconnected(float timeTaken)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.reconnected.ToString());
			val.AddParam("timeTaken", (object)timeTaken);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendCubesUnlocked(uint techPointsCost, string cubeNameKey, int techsUnlocked)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.cubesUnlocked.ToString());
			val.AddParam("techPoint", (object)techPointsCost);
			val.AddParam("cubeName", (object)cubeNameKey);
			val.AddParam("techsUnlocked", (object)techsUnlocked);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendCampaignWaveSummary(string campaignNameKey, int campaignDifficulty, int waveNumber, int waveDuration, int livesLost, string result, int kills)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.campaignWaveSummary.ToString());
			val.AddParam("campaignName", (object)campaignNameKey);
			val.AddParam("campaignDifficulty", (object)campaignDifficulty);
			val.AddParam("waveNumber", (object)waveNumber);
			val.AddParam("waveDuration", (object)waveDuration);
			val.AddParam("livesLost", (object)livesLost);
			val.AddParam("result", (object)result);
			val.AddParam("kills", (object)kills);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendTierRankUp(uint tier, string rank)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.tierRankUp.ToString());
			val.AddParam("tier", (object)tier);
			val.AddParam("rank", (object)rank);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendItemBought(string item, string itemType, string currency, int cost, string context, int discount)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.itemBought.ToString());
			val.AddParam("itemName", (object)item);
			val.AddParam("itemType", (object)itemType);
			val.AddParam("currency", (object)currency);
			val.AddParam("cost", (object)cost);
			val.AddParam("context", (object)context);
			val.AddParam("discount", (object)discount);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendItemStocked(string item, string itemType, string currency, string context, string restock, bool locked, int discount)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.itemStocked.ToString());
			val.AddParam("itemName", (object)item);
			val.AddParam("itemType", (object)itemType);
			val.AddParam("currency", (object)currency);
			val.AddParam("context", (object)context);
			val.AddParam("restock", (object)restock);
			val.AddParam("locked", (object)locked);
			val.AddParam("discount", (object)discount);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendItemShopVisited(string context)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.itemShopVisited.ToString());
			val.AddParam("context", (object)context);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendXpEarned(int earned, int userXP, int? roboPassXP, int userLevel, int premiumBonus, string source, string sourceDetail)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.xpEarned.ToString());
			val.AddParam("earned", (object)earned);
			val.AddParam("userXP", (object)userXP);
			val.AddParam("userLevel", (object)userLevel);
			val.AddParam("premiumBonus", (object)premiumBonus);
			val.AddParam("sourceName", (object)source);
			val.AddParam("sourceDetail", (object)sourceDetail);
			if (roboPassXP.HasValue)
			{
				val.AddParam("roboPassXP", (object)roboPassXP.Value);
			}
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendCurrencyEarned(string currency, int earned, long balance, int premiumBonus, string source, string sourceDetail)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.currencyEarned.ToString());
			val.AddParam("currency", (object)currency);
			val.AddParam("earned", (object)earned);
			val.AddParam("balance", (object)balance);
			val.AddParam("premiumBonus", (object)premiumBonus);
			val.AddParam("sourceName", (object)source);
			val.AddParam("sourceDetail", (object)sourceDetail);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendCurrencySpent(string currency, int spent, long balance, string sink, string sinkDetail)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.currencySpent.ToString());
			val.AddParam("currency", (object)currency);
			val.AddParam("spent", (object)spent);
			val.AddParam("balance", (object)balance);
			val.AddParam("sink", (object)sink);
			val.AddParam("sinkDetail", (object)sinkDetail);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendQuestAdded(string questID, int activeQuests)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.questAdded.ToString());
			val.AddParam("questID", (object)questID);
			val.AddParam("activeQuests", (object)activeQuests);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendQuestCompleted(string questID, int activeQuests)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.questCompleted.ToString());
			val.AddParam("questID", (object)questID);
			val.AddParam("activeQuests", (object)activeQuests);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendQuestRerolled(string questID, int activeQuests)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.questRerolled.ToString());
			val.AddParam("questID", (object)questID);
			val.AddParam("activeQuests", (object)activeQuests);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendPurchaseFunnelStart(string step, string context, string eventId)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.purchaseFunnel.ToString(), eventId);
			val.AddParam("step", (object)step);
			val.AddParam("context", (object)context);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendPurchaseFunnelStep(string step, string context, string startEventId)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.purchaseFunnel.ToString());
			val.AddParam("step", (object)step);
			val.AddParam("context", (object)context);
			val.AddParam("funnelStartEventID", (object)startEventId);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendRoboPassGradeUp(string season, int grade)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.roboPassGradeUp.ToString());
			val.AddParam("season", (object)season);
			val.AddParam("grade", (object)grade);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void SendRoboPassRewardCollected(string prize, int amount, string season, bool roboPassPlus, int grade)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (isRunning)
		{
			GameEvent val = new GameEvent(DeltaDNAEventName.roboPassRewardCollected.ToString());
			val.AddParam("prize", (object)prize);
			val.AddParam("amount", (object)amount);
			val.AddParam("season", (object)season);
			val.AddParam("roboPassPlus", (object)roboPassPlus);
			val.AddParam("grade", (object)grade);
			Singleton<DDNA>.get_Instance().RecordEvent<GameEvent>(val);
		}
	}

	public static void DeltaDNAEndSession()
	{
		if (isRunning)
		{
			isRunning = false;
			Singleton<DDNA>.get_Instance().StopSDK();
		}
	}

	private static void SetIsNewToDeltaDNA(string username, bool isNewUser)
	{
		string userID = Singleton<DDNA>.get_Instance().get_UserID();
		if (!isNewUser && userID != username && !string.IsNullOrEmpty(username))
		{
			PlayerPrefs.SetString("DDSDK_USER_ID", username);
			PlayerPrefs.Save();
		}
	}
}
