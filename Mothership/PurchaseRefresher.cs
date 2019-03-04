using Services.Analytics;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class PurchaseRefresher
	{
		private const int MAX_NUM_RETRIES = 10;

		private const float TIME_BETWEEN_RETRIES = 5f;

		private int _numRetries;

		private long _ccBalance;

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		public PremiumMembership premiumMembership
		{
			private get;
			set;
		}

		[Inject]
		public ICubeInventory cubeInventory
		{
			private get;
			set;
		}

		[Inject]
		public LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		public ICurrenciesTracker currenciesTracker
		{
			private get;
			set;
		}

		[Inject]
		public PurchaseConfirmedController purchaseConfirmedController
		{
			private get;
			set;
		}

		[Inject]
		public IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		public ReloadRobopassObservable reloadRobopassObservable
		{
			private get;
			set;
		}

		[Inject]
		public RealMoneyStoreDataSource realMoneyStoreDataSource
		{
			private get;
			set;
		}

		public void StartPollForPurchases(bool purchaseTriggered)
		{
			_numRetries = ((!purchaseTriggered) ? 1 : 10);
			TaskRunner.get_Instance().Run(PollForPurchases());
		}

		private IEnumerator PollForPurchases()
		{
			ShowLoadingScreen();
			while (_numRetries-- > 0)
			{
				yield return RequestDataAsTask();
				if (_numRetries > 0)
				{
					yield return (object)new WaitForSecondsEnumerator(5f);
				}
			}
			HideLoadingScreen();
		}

		public IEnumerator RequestDataAsTask()
		{
			ILoadPurchasesRequest service = serviceFactory.Create<ILoadPurchasesRequest>();
			TaskService<FasterList<PurchaseRequestData>> task = new TaskService<FasterList<PurchaseRequestData>>(service);
			yield return new HandleTaskServiceWithError(task, ShowLoadingScreen, HideLoadingScreen).GetEnumerator();
			if (task.succeeded)
			{
				yield return OnPurchasesLoaded(task.result);
			}
		}

		private IEnumerator OnPurchasesLoaded(FasterList<PurchaseRequestData> purchases)
		{
			Console.Log("LoadPurchases retry: done " + _numRetries);
			if (purchases.get_Count() <= 0)
			{
				yield break;
			}
			_numRetries = 0;
			currenciesTracker.RetrieveCurrentWallet(delegate(Wallet wallet)
			{
				_ccBalance = wallet.CosmeticCreditsBalance;
			});
			for (int i = 0; i < purchases.get_Count(); i++)
			{
				PurchaseRequestData purchaseRequestData = purchases.get_Item(i);
				ShopItemType shopItemType = purchaseRequestData.ShopItemType;
				switch (shopItemType)
				{
				case ShopItemType.Premium:
				{
					PremiumPurchaseResponse premiumPurchaseResponse = purchaseRequestData.premiumPurchaseResponse;
					premiumMembership.UpdatePremiumPurchase(premiumPurchaseResponse);
					purchaseConfirmedController.ShowPremiumPurchased(premiumPurchaseResponse.numPremiumDaysAwarded);
					break;
				}
				case ShopItemType.PremiumForLife:
					premiumMembership.UpdatePremiumPurchase(purchaseRequestData.premiumPurchaseResponse);
					purchaseConfirmedController.ShowLifeTimePremiumPurchased();
					break;
				case ShopItemType.Cube:
					cubeInventory.RefreshAndForget();
					break;
				case ShopItemType.CosmeticCredits:
					yield return currenciesTracker.RefreshUserWalletEnumerator();
					yield return HandleAnalytics(purchaseRequestData.PurchasedCCList);
					purchaseConfirmedController.ShowCosmeticCreditsPurchased(purchaseRequestData.TotalPurchasedCC);
					break;
				case ShopItemType.RoboPass:
					reloadRobopassObservable.Dispatch();
					purchaseConfirmedController.ShowRoboPassPurchased();
					break;
				default:
					Console.LogError("Shop item type '" + shopItemType + "' is of unknown type.");
					break;
				}
			}
			yield return realMoneyStoreDataSource.LoadData();
		}

		private void ShowLoadingScreen()
		{
			loadingIconPresenter.NotifyLoading("PurchaseScreen");
		}

		private void HideLoadingScreen()
		{
			loadingIconPresenter.NotifyLoadingDone("PurchaseScreen");
		}

		private IEnumerator HandleAnalytics(FasterList<int> purchasedCCList)
		{
			loadingIconPresenter.NotifyLoading("HandleAnalytics");
			for (int i = 0; i < purchasedCCList.get_Count(); i++)
			{
				_ccBalance += purchasedCCList.get_Item(i);
				LogPlayerCurrencyEarnedDependency playerCurrencyEarnedDependency = new LogPlayerCurrencyEarnedDependency(CurrencyType.CosmeticCredits.ToString(), purchasedCCList.get_Item(i), _ccBalance, 0, "IAP", string.Empty);
				TaskService logPlayerCurrencyEarnedRequest = analyticsRequestFactory.Create<ILogPlayerCurrencyEarnedRequest, LogPlayerCurrencyEarnedDependency>(playerCurrencyEarnedDependency).AsTask();
				yield return logPlayerCurrencyEarnedRequest;
				if (!logPlayerCurrencyEarnedRequest.succeeded)
				{
					Console.LogError("Log Player Earned Currency Request failed. " + logPlayerCurrencyEarnedRequest.behaviour.exceptionThrown);
				}
			}
			loadingIconPresenter.NotifyLoadingDone("HandleAnalytics");
		}
	}
}
