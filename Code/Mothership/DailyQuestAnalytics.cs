using Authentication;
using Services.Analytics;
using SocialServiceLayer;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Mothership
{
	internal class DailyQuestAnalytics
	{
		[Inject]
		private LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		[Inject]
		private IAnalyticsRequestFactory analyticsRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private IServiceRequestFactory serviceRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private ISocialRequestFactory socialRequestFactory
		{
			get;
			set;
		}

		[Inject]
		private PremiumMembership premiumMembership
		{
			get;
			set;
		}

		internal IEnumerator HandleAddQuestAnalytics(List<Quest> activeQuests)
		{
			if (activeQuests.Count <= 0)
			{
				yield break;
			}
			string empty = string.Empty;
			string username = User.Username;
			char separator = ';';
			string oldQuestsString = PlayerPrefs.GetString("analytics_activeQuests_" + username);
			string[] oldQuests = oldQuestsString.Split(separator);
			string[] newActiveQuests = new string[activeQuests.Count];
			int i;
			for (i = 0; i < activeQuests.Count; i++)
			{
				if (!Array.Exists(oldQuests, (string element) => element == activeQuests[i].questID))
				{
					LogQuestAddedDependency questAddedDependency = new LogQuestAddedDependency(activeQuests[i].questID, activeQuests.Count);
					TaskService logQuestAddedRequest = analyticsRequestFactory.Create<ILogQuestAddedRequest, LogQuestAddedDependency>(questAddedDependency).AsTask();
					yield return logQuestAddedRequest;
					if (!logQuestAddedRequest.succeeded)
					{
						Console.LogError("Log Quest Added Request failed. " + logQuestAddedRequest.behaviour.exceptionThrown);
					}
				}
				newActiveQuests[i] = activeQuests[i].questID;
			}
			string newActiveQuestsString = string.Join(separator.ToString(), newActiveQuests);
			PlayerPrefs.SetString("analytics_activeQuests_" + username, newActiveQuestsString);
		}

		internal IEnumerator HandleReplaceQuestAnalytics(int activeQuestsCount, string questID)
		{
			loadingIconPresenter.NotifyLoading("HandleAnalytics");
			LogQuestRerolledDependency questRerolledDependency = new LogQuestRerolledDependency(questID, activeQuestsCount);
			TaskService logQuestRerolledRequest = analyticsRequestFactory.Create<ILogQuestRerolledRequest, LogQuestRerolledDependency>(questRerolledDependency).AsTask();
			yield return logQuestRerolledRequest;
			if (!logQuestRerolledRequest.succeeded)
			{
				Console.LogError("Log Quest Rerolled Request failed. " + logQuestRerolledRequest.behaviour.exceptionThrown);
			}
			loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		internal IEnumerator HandleCompleteQuestAnalytics(PlayerDailyQuestsData playerDailyQuestsData, long previousRobitsBalance, int previousLevel)
		{
			loadingIconPresenter.NotifyLoading("HandleAnalytics");
			string empty = string.Empty;
			string username = User.Username;
			TaskService<uint[]> xpRequest = serviceRequestFactory.Create<ILoadTotalXPRequest>().AsTask();
			yield return xpRequest;
			if (!xpRequest.succeeded)
			{
				Console.LogError("Load Total Xp Request failed. " + xpRequest.behaviour.exceptionThrown);
				loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			int updatedXP = Convert.ToInt32(xpRequest.result[0]);
			ILoadPlayerRoboPassSeasonRequest loadPlayerRoboPassSeasonReq = serviceRequestFactory.Create<ILoadPlayerRoboPassSeasonRequest>();
			loadPlayerRoboPassSeasonReq.ClearCache();
			TaskService<PlayerRoboPassSeasonData> loadPlayerRoboPassSeasonTS = loadPlayerRoboPassSeasonReq.AsTask();
			yield return loadPlayerRoboPassSeasonTS;
			if (!loadPlayerRoboPassSeasonTS.succeeded)
			{
				Console.LogError("Failed to get RoboPass player season data");
				loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			PlayerRoboPassSeasonData playerRoboPassSeasonData = loadPlayerRoboPassSeasonTS.result;
			int? roboPassXP = null;
			if (playerRoboPassSeasonData != null)
			{
				roboPassXP = playerRoboPassSeasonData.xpFromSeasonStart + playerRoboPassSeasonData.deltaXpToShow;
			}
			int completedQuestCount = playerDailyQuestsData.completedQuests.Count;
			int activeQuestCount = playerDailyQuestsData.playerQuests.Count;
			long updatedRobitsBalance = previousRobitsBalance;
			for (int i = 0; i < completedQuestCount; i++)
			{
				int premiumXP = premiumMembership.hasSubscription ? playerDailyQuestsData.completedQuests[i].premiumXP : 0;
				LogPlayerXpEarnedDependency playerXpEarnedDependency = new LogPlayerXpEarnedDependency(playerDailyQuestsData.completedQuests[i].xp + premiumXP, updatedXP, roboPassXP, previousLevel, premiumXP, "DailyQuest", playerDailyQuestsData.completedQuests[i].questID);
				TaskService logPlayerXpEarnedRequest = analyticsRequestFactory.Create<ILogPlayerXpEarnedRequest, LogPlayerXpEarnedDependency>(playerXpEarnedDependency).AsTask();
				yield return logPlayerXpEarnedRequest;
				if (!logPlayerXpEarnedRequest.succeeded)
				{
					Console.LogError("Log Player Xp Request failed. " + logPlayerXpEarnedRequest.behaviour.exceptionThrown);
				}
				int premiumRobits = premiumMembership.hasSubscription ? playerDailyQuestsData.completedQuests[i].premiumRobits : 0;
				int earnedRobits = playerDailyQuestsData.completedQuests[i].robits + premiumRobits;
				updatedRobitsBalance += earnedRobits;
				LogPlayerCurrencyEarnedDependency playerCurrencyEarnedDependency = new LogPlayerCurrencyEarnedDependency(CurrencyType.Robits.ToString(), earnedRobits, updatedRobitsBalance, premiumRobits, "DailyQuest", playerDailyQuestsData.completedQuests[i].questID);
				TaskService logPlayerCurrencyEarnedRequest = analyticsRequestFactory.Create<ILogPlayerCurrencyEarnedRequest, LogPlayerCurrencyEarnedDependency>(playerCurrencyEarnedDependency).AsTask();
				yield return logPlayerCurrencyEarnedRequest;
				if (!logPlayerCurrencyEarnedRequest.succeeded)
				{
					Console.LogError("Log Player Earned Currency Request failed. " + logPlayerCurrencyEarnedRequest.behaviour.exceptionThrown);
				}
				int stillActiveCount = activeQuestCount + completedQuestCount - (i + 1);
				LogQuestCompletedDependency questCompletedDependency = new LogQuestCompletedDependency(playerDailyQuestsData.completedQuests[i].questID, stillActiveCount);
				TaskService logQuestCompletedRequest = analyticsRequestFactory.Create<ILogQuestCompletedRequest, LogQuestCompletedDependency>(questCompletedDependency).AsTask();
				yield return logQuestCompletedRequest;
				if (!logQuestCompletedRequest.succeeded)
				{
					Console.LogError("Log Quest Completed Request failed. " + logQuestCompletedRequest.behaviour.exceptionThrown);
				}
			}
			loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}
	}
}
