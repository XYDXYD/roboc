using Services.Analytics;
using Services.TechTree;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	public class InitialLoginAnalytics
	{
		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialRequestFactory socialRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ICurrenciesTracker currenciesTracker
		{
			private get;
			set;
		}

		[Inject]
		internal ChatSettings chatSettings
		{
			private get;
			set;
		}

		[Inject]
		internal CapFrameRateSettings capFrameRateSettings
		{
			private get;
			set;
		}

		[Inject]
		internal MouseSettings mouseSettings
		{
			private get;
			set;
		}

		[Inject]
		internal AdvancedRobotEditSettings advancedRobotEditSettings
		{
			private get;
			set;
		}

		[Inject]
		internal SocialSettings socialSettings
		{
			private get;
			set;
		}

		[Inject]
		internal PremiumMembership premiumMembership
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public void SendLoggedInEvents()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)SendEvents);
		}

		private IEnumerator SendEvents()
		{
			yield return PostLoggedInEvents();
			yield return PostLoggedInSettingsEvents();
		}

		private IEnumerator PostLoggedInEvents()
		{
			int playerLevel = -1;
			uint playerXP = 0u;
			long robits = -1L;
			long cosmeticCredits = -1L;
			bool isDeveloper = false;
			int totalFriends = 0;
			string clanName = string.Empty;
			AnalyticsPremiumSubscriptionType premiumType = AnalyticsPremiumSubscriptionType.None;
			yield return PlayerLevelHelper.LoadCurrentPlayerLevel(serviceFactory, delegate(PlayerLevelAndProgress playerLevelData)
			{
				playerLevel = (int)playerLevelData.playerLevel;
			}, delegate
			{
				Console.Log("Could not load playerLevel for analytics");
			});
			if (playerLevel == -1)
			{
				yield return null;
			}
			ILoadTotalXPRequest xpRequest = serviceFactory.Create<ILoadTotalXPRequest>();
			TaskService<uint[]> xpTask = new TaskService<uint[]>(xpRequest);
			yield return xpTask;
			if (xpTask.succeeded)
			{
				playerXP = xpTask.result[0];
			}
			yield return currenciesTracker.RefreshUserWalletEnumerator();
			currenciesTracker.RetrieveCurrentWallet(delegate(Wallet userWallet)
			{
				robits = userWallet.RobitsBalance;
				cosmeticCredits = userWallet.CosmeticCreditsBalance;
			});
			IGetAccountRightsRequest accountRightsRequest = serviceFactory.Create<IGetAccountRightsRequest>();
			TaskService<AccountRights> accountRightsTask = new TaskService<AccountRights>(accountRightsRequest);
			yield return accountRightsTask;
			if (accountRightsTask.succeeded)
			{
				isDeveloper = accountRightsTask.result.Developer;
			}
			IGetFriendListRequest getFriendsRequest = socialRequestFactory.Create<IGetFriendListRequest>();
			TaskService<GetFriendListResponse> getFriendsTask = new TaskService<GetFriendListResponse>(getFriendsRequest);
			yield return getFriendsTask;
			if (getFriendsTask.succeeded)
			{
				totalFriends = getFriendsTask.result.friendsList.Count;
			}
			IGetMyClanInfoRequest clanInfoServiceRequest = socialRequestFactory.Create<IGetMyClanInfoRequest>();
			TaskService<ClanInfo> clanInfoTask = new TaskService<ClanInfo>(clanInfoServiceRequest);
			yield return clanInfoTask;
			if (clanInfoTask.succeeded && clanInfoTask.result != null)
			{
				clanName = clanInfoTask.result.ClanName;
			}
			while (!premiumMembership.Loaded())
			{
				yield return null;
			}
			if (premiumMembership.hasSubscription)
			{
				premiumType = ((!premiumMembership.hasPremiumForLife) ? AnalyticsPremiumSubscriptionType.Days : AnalyticsPremiumSubscriptionType.Life);
			}
			TaskService<Dictionary<string, TechTreeItemData>> getTechTreeDataRequest = serviceFactory.Create<IGetTechTreeDataRequest>().AsTask();
			yield return getTechTreeDataRequest;
			if (!getTechTreeDataRequest.succeeded)
			{
				Console.LogError("Get Tech Tree Data Request failed. " + getTechTreeDataRequest.behaviour.exceptionThrown);
				yield break;
			}
			int unlockedTechs = 0;
			Dictionary<string, TechTreeItemData>.Enumerator enumerator = getTechTreeDataRequest.result.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.isUnlocked)
				{
					unlockedTechs++;
				}
			}
			ILoadPlayerRoboPassSeasonRequest loadPlayerRoboPassSeasonReq = serviceFactory.Create<ILoadPlayerRoboPassSeasonRequest>();
			loadPlayerRoboPassSeasonReq.ClearCache();
			TaskService<PlayerRoboPassSeasonData> loadPlayerRoboPassSeasonTS = loadPlayerRoboPassSeasonReq.AsTask();
			yield return loadPlayerRoboPassSeasonTS;
			if (!loadPlayerRoboPassSeasonTS.succeeded)
			{
				Console.LogError("Failed to get RoboPass player season data");
				yield break;
			}
			PlayerRoboPassSeasonData playerRoboPassSeasonData = loadPlayerRoboPassSeasonTS.result;
			int? roboPassXP = null;
			bool? roboPassPlus = null;
			if (playerRoboPassSeasonData != null)
			{
				roboPassXP = playerRoboPassSeasonData.xpFromSeasonStart + playerRoboPassSeasonData.deltaXpToShow;
				roboPassPlus = playerRoboPassSeasonData.hasDeluxe;
			}
			IGetABTestGroupRequest abTestRequest = serviceFactory.Create<IGetABTestGroupRequest>();
			TaskService<ABTestData> abTestTask = abTestRequest.AsTask();
			yield return abTestTask;
			if (!abTestTask.succeeded)
			{
				throw new Exception("Retrieve AB test group failed", abTestTask.behaviour.exceptionThrown);
			}
			ABTestData abData = abTestTask.result;
			LogOnLoggedInPlayerDataDependency onLoggedInPlayerDataDependency = new LogOnLoggedInPlayerDataDependency(playerLevel, playerXP, robits, cosmeticCredits, isDeveloper, totalFriends, clanName, premiumType, unlockedTechs, roboPassXP, roboPassPlus, abData.ABTest, abData.ABTestGroup);
			TaskService logOnLoggedInPlayerDataRequest = analyticsRequestFactory.Create<ILogOnLoggedInPlayerDataRequest, LogOnLoggedInPlayerDataDependency>(onLoggedInPlayerDataDependency).AsTask();
			yield return logOnLoggedInPlayerDataRequest;
			if (!logOnLoggedInPlayerDataRequest.succeeded)
			{
				Console.LogError("Log OnLoggedInPlayerData Request failed. " + logOnLoggedInPlayerDataRequest.behaviour.exceptionThrown);
			}
		}

		private IEnumerator PostLoggedInSettingsEvents()
		{
			Resolution currentResolution = Screen.get_currentResolution();
			string resolution = ((object)currentResolution).ToString();
			string gfx = SystemInfo.get_graphicsDeviceName();
			string processorType = SystemInfo.get_processorType();
			int memorySize = SystemInfo.get_systemMemorySize();
			int shaderLevel = SystemInfo.get_graphicsShaderLevel();
			bool fullScreen = Screen.get_fullScreen();
			bool settingsLoaded = false;
			chatSettings.RegisterOnLoaded(delegate
			{
				settingsLoaded = true;
			});
			while (!settingsLoaded)
			{
				yield return null;
			}
			capFrameRateSettings.InitialiseFrameRate();
			bool capFrameRateEnabled = capFrameRateSettings.capEnabled;
			int capFrameRateAmount = capFrameRateSettings.cappedFrameRateAmount;
			bool zoomMode = mouseSettings.IsToggleZoom();
			bool invertY = mouseSettings.IsInvertY();
			bool showCenterOfMass = advancedRobotEditSettings.centerOfMass;
			bool blockFriendClanInvites = socialSettings.IsBlockFriendInvites();
			bool acceptFriendClanOnlyInvites = socialSettings.GetAcceptPartyInvitesFromFriendsAndClanOnlySetting();
			string language = StringTableBase<StringTable>.Instance.language;
			LogOnLoggedInSettingsDependency onLoggedInSettingsDependency = new LogOnLoggedInSettingsDependency(resolution, gfx, fullScreen, capFrameRateEnabled, capFrameRateAmount, zoomMode, invertY, showCenterOfMass, blockFriendClanInvites, acceptFriendClanOnlyInvites, language, processorType, memorySize, shaderLevel);
			TaskService logOnLoggedInSettingsRequest = analyticsRequestFactory.Create<ILogOnLoggedInSettingsRequest, LogOnLoggedInSettingsDependency>(onLoggedInSettingsDependency).AsTask();
			yield return logOnLoggedInSettingsRequest;
			if (!logOnLoggedInSettingsRequest.succeeded)
			{
				Console.LogError("Log OnLoggedInSettings Request failed. " + logOnLoggedInSettingsRequest.behaviour.exceptionThrown);
			}
		}
	}
}
