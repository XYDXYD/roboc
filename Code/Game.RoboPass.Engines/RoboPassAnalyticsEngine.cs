using Game.RoboPass.Components;
using Game.RoboPass.EntityViews;
using Mothership;
using Services.Analytics;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Game.RoboPass.Engines
{
	internal class RoboPassAnalyticsEngine : SingleEntityViewEngine<RoboPassSeasonDataEntityView>, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private readonly IAnalyticsRequestFactory _analyticsRequestFactory;

		private readonly LoadingIconPresenter _loadingIconPresenter;

		private readonly ICurrenciesTracker _currenciesTracker;

		private readonly ReloadRobopassObserver _reloadRobopassObserver;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RoboPassAnalyticsEngine(IAnalyticsRequestFactory analyticsRequestFactory, LoadingIconPresenter loadingIconPresenter, ICurrenciesTracker currenciesTracker, ReloadRobopassObserver reloadRobopassObserver)
		{
			_analyticsRequestFactory = analyticsRequestFactory;
			_loadingIconPresenter = loadingIconPresenter;
			_currenciesTracker = currenciesTracker;
			_reloadRobopassObserver = reloadRobopassObserver;
		}

		public void Ready()
		{
			_reloadRobopassObserver.AddAction((Action)HandleOnRobopassDeluxeBought);
		}

		public void OnFrameworkDestroyed()
		{
			_reloadRobopassObserver.RemoveAction((Action)HandleOnRobopassDeluxeBought);
		}

		protected override void Add(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonPlayerInfoComponent.reachedNewGrade.NotifyOnValueSet((Action<int, bool>)OnReachedNewGrade);
		}

		protected override void Remove(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonPlayerInfoComponent.reachedNewGrade.StopNotify((Action<int, bool>)OnReachedNewGrade);
		}

		private void OnReachedNewGrade(int id, bool reachedNewGrade)
		{
			if (reachedNewGrade)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)HandleAnalytics);
			}
		}

		private void HandleOnRobopassDeluxeBought()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<RoboPassSeasonDataEntityView> val = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>();
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent = val.get_Item(0).roboPassSeasonInfoComponent;
			IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent = val.get_Item(0).roboPassSeasonPlayerInfoComponent;
			_currenciesTracker.RetrieveCurrentWallet(delegate(Wallet wallet)
			{
				for (int i = 0; i <= roboPassSeasonPlayerInfoComponent.currentGradeIndex; i++)
				{
					TaskRunner.get_Instance().Run(HandleRewardCollectedAnalytics(roboPassSeasonInfoComponent, roboPassSeasonPlayerInfoComponent, wallet.RobitsBalance, wallet.CosmeticCreditsBalance, i, logOnlyDeluxeRewards: true));
				}
			});
		}

		private IEnumerator HandleAnalytics()
		{
			_loadingIconPresenter.NotifyLoading("HandleAnalytics");
			FasterReadOnlyList<RoboPassSeasonDataEntityView> seasonDataEntityViews = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>();
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent = seasonDataEntityViews.get_Item(0).roboPassSeasonInfoComponent;
			IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent = seasonDataEntityViews.get_Item(0).roboPassSeasonPlayerInfoComponent;
			yield return HandleGradeUpAnalytics(roboPassSeasonInfoComponent, roboPassSeasonPlayerInfoComponent);
			_currenciesTracker.RetrieveCurrentWallet(delegate(Wallet wallet)
			{
				TaskRunner.get_Instance().Run(HandleRewardCollectedAnalytics(roboPassSeasonInfoComponent, roboPassSeasonPlayerInfoComponent, wallet.RobitsBalance, wallet.CosmeticCreditsBalance, roboPassSeasonPlayerInfoComponent.currentGradeIndex, logOnlyDeluxeRewards: false));
			});
			_loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}

		private IEnumerator HandleGradeUpAnalytics(IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent, IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent)
		{
			int currentGradeIndex = roboPassSeasonPlayerInfoComponent.currentGradeIndex;
			LogPlayerRoboPassGradeUpDependency roboPassGradeUpDependency = new LogPlayerRoboPassGradeUpDependency(roboPassSeasonInfoComponent.robopassSeasonNameKey, currentGradeIndex + 1);
			TaskService logPlayerRoboPassGradeUpRequest = _analyticsRequestFactory.Create<ILogPlayerRoboPassGradeUpRequest, LogPlayerRoboPassGradeUpDependency>(roboPassGradeUpDependency).AsTask();
			yield return logPlayerRoboPassGradeUpRequest;
			if (!logPlayerRoboPassGradeUpRequest.succeeded)
			{
				Console.LogError("Log Player RoboPass Grade Up Request failed. " + logPlayerRoboPassGradeUpRequest.behaviour.exceptionThrown);
			}
		}

		private IEnumerator HandleRewardCollectedAnalytics(IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent, IRoboPassSeasonPlayerInfoComponent roboPassSeasonPlayerInfoComponent, long robitsBalance, long cosmeticCreditsBalance, int currentGradeIndex, bool logOnlyDeluxeRewards)
		{
			RoboPassSeasonRewardData[] rewards = roboPassSeasonInfoComponent.gradesRewards[currentGradeIndex];
			if (rewards == null || rewards.Length <= 0)
			{
				yield break;
			}
			RoboPassSeasonRewardData[] array = rewards;
			foreach (RoboPassSeasonRewardData rewardData in array)
			{
				if ((rewardData.isDeluxe || logOnlyDeluxeRewards) && ((!logOnlyDeluxeRewards && !roboPassSeasonPlayerInfoComponent.hasDeluxe) || !rewardData.isDeluxe))
				{
					continue;
				}
				int count = 1;
				if (rewardData.items.Length == 1 && (rewardData.items[0].category == RoboPassSeasonRewardCategory.Robits || rewardData.items[0].category == RoboPassSeasonRewardCategory.CosmeticCredits))
				{
					count = rewardData.items[0].count;
				}
				LogPlayerRoboPassRewardCollectedDependency rewardCollectedDependency = new LogPlayerRoboPassRewardCollectedDependency(rewardData.rewardNameKey, count, roboPassSeasonInfoComponent.robopassSeasonNameKey, rewardData.isDeluxe, currentGradeIndex + 1);
				TaskService logPlayerRoboPassRewardCollectedRequest = _analyticsRequestFactory.Create<ILogPlayerRoboPassRewardCollectedRequest, LogPlayerRoboPassRewardCollectedDependency>(rewardCollectedDependency).AsTask();
				yield return logPlayerRoboPassRewardCollectedRequest;
				if (!logPlayerRoboPassRewardCollectedRequest.succeeded)
				{
					Console.LogError("Log Player RoboPass Reward Collected Request failed. " + logPlayerRoboPassRewardCollectedRequest.behaviour.exceptionThrown);
				}
				for (int i = 0; i < rewardData.items.Length; i++)
				{
					ItemData item = rewardData.items[i];
					if (item.category == RoboPassSeasonRewardCategory.Robits || item.category == RoboPassSeasonRewardCategory.CosmeticCredits)
					{
						string sourceDetail = rewardData.rewardNameKey + " " + (currentGradeIndex + 1) + " " + ((!rewardData.isDeluxe) ? "Free" : "Plus");
						long balance;
						if (item.category == RoboPassSeasonRewardCategory.Robits)
						{
							robitsBalance += count;
							balance = robitsBalance;
						}
						else
						{
							cosmeticCreditsBalance += count;
							balance = cosmeticCreditsBalance;
						}
						LogPlayerCurrencyEarnedDependency currencyEarnedDependency = new LogPlayerCurrencyEarnedDependency(item.category.ToString(), item.count, balance, 0, "roboPass", sourceDetail);
						TaskService logPlayerCurrencyEarnedRequest = _analyticsRequestFactory.Create<ILogPlayerCurrencyEarnedRequest, LogPlayerCurrencyEarnedDependency>(currencyEarnedDependency).AsTask();
						yield return logPlayerCurrencyEarnedRequest;
						if (!logPlayerCurrencyEarnedRequest.succeeded)
						{
							Console.LogError("Log Player Currency Earned Request failed. " + logPlayerCurrencyEarnedRequest.behaviour.exceptionThrown);
						}
					}
				}
			}
		}
	}
}
