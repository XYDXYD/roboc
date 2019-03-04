using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using Utility;

namespace Mothership
{
	internal class RealMoneyStoreCardController : IRealMoneyStoreCardController, IInitialize, IWaitForFrameworkDestruction
	{
		public const string REAL_MONEY_STORE_NAME_PREFIX = "strRealMoneyStoreName_";

		public const string REAL_MONEY_STORE_DESC_PREFIX = "strRealMoneyStoreDesc_";

		private int _slotId;

		private RealMoneyStoreItemCardView _view;

		private RealMoneyStoreSlotDisplayType _slotType;

		private IRealMoneyStoreItemDataSource _dataSource;

		[Inject]
		internal IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		[Inject]
		internal PremiumMembership premiumMembership
		{
			get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			premiumMembership.onSubscriptionActivated += delegate
			{
				RefreshDueToPremiumStatusChange();
			};
			premiumMembership.onSubscriptionExpired += RefreshDueToPremiumStatusChange;
		}

		private void RefreshDueToPremiumStatusChange()
		{
			if (_dataSource != null)
			{
				HandleDataChanged(dataHasChanged: true);
			}
		}

		public void RegisterView(RealMoneyStoreItemCardView storeCardView, int slotId, RealMoneyStoreSlotDisplayType slotType)
		{
			_view = storeCardView;
			_slotId = slotId;
			_slotType = slotType;
		}

		public void SetDataSource(IRealMoneyStoreItemDataSource dataSource)
		{
			_dataSource = dataSource;
			_dataSource.OnDataChanged += HandleDataChanged;
		}

		public void OnFrameworkDestroyed()
		{
			if (_dataSource != null)
			{
				_dataSource.OnDataChanged -= HandleDataChanged;
			}
		}

		private void HandleDataChanged(bool dataHasChanged)
		{
			if (_slotId >= _dataSource.GetDataItemsCount(_slotType))
			{
				_view.get_gameObject().SetActive(false);
				return;
			}
			RealMoneyStoreItemBundle dataItem = _dataSource.GetDataItem(_slotId, _slotType);
			_view.SetItemName(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreName_" + dataItem.ItemSku));
			_view.SetPriceText(dataItem.currencyString);
			bool flag = false;
			int numberOfDays = 0;
			foreach (RealMoneyStoreItem item in dataItem.Items)
			{
				RealMoneyStoreItem current = item;
				if (current.StoreItemType == RealMoneyStoreItemType.PremiumForLife)
				{
					flag = true;
				}
				if (current.StoreItemType == RealMoneyStoreItemType.Premium)
				{
					numberOfDays = current.CountOfItems;
				}
			}
			if (_slotType == RealMoneyStoreSlotDisplayType.PremiumRow)
			{
				if (flag)
				{
					_view.ShowLifetimePremium();
				}
				else
				{
					_view.ShowTimeLimitedPremium(numberOfDays);
				}
			}
			_view.ResetBannerOverlays();
			_view.ShowOriginalPriceWidget(visible: false);
			if (dataItem.oldPriceForCheck != dataItem.priceForCheck)
			{
				int discountPercentage = (int)Math.Round(100f - dataItem.priceForCheck / dataItem.oldPriceForCheck * 100f);
				_view.ShowDiscountOverlay(discountPercentage);
				_view.ShowOriginalPriceWidget(visible: true);
				_view.SetOriginalPrice(dataItem.oldCurrencyString.ToString());
			}
			else if (dataItem.bestValueFlag)
			{
				_view.ShowBestValueOverlay();
			}
			else if (dataItem.mostPopularFlag)
			{
				_view.ShowMostPopularOverlay();
			}
			else if (dataItem.additionalValue > 0)
			{
				_view.ShowSavingOverlay(dataItem.additionalValue);
			}
			if (_slotType != RealMoneyStoreSlotDisplayType.CosmeticCreditsRow)
			{
				_view.SetAlreadyOwned(status: true);
				TaskRunner.get_Instance().Run(DetermineAlreadyOwnedStatus());
			}
			else
			{
				_view.SetAlreadyOwned(status: false);
			}
			TaskRunner.get_Instance().Run(DetermineAvailableStatus());
			_view.SetSpriteBySku(dataItem.ItemSku);
			_view.get_gameObject().SetActive(true);
		}

		private IEnumerator DetermineAvailableStatus()
		{
			RealMoneyStoreItemBundle dataItem = _dataSource.GetDataItem(_slotId, _slotType);
			foreach (RealMoneyStoreItem item2 in dataItem.Items)
			{
				RealMoneyStoreItem item = item2;
				bool available = true;
				if (item.StoreItemType == RealMoneyStoreItemType.RoboPass)
				{
					IEnumerator result = IsRoboPassAvailable();
					yield return result;
					available = (bool)result.Current;
				}
				_view.SetAvailable(available);
			}
		}

		private IEnumerator IsRoboPassAvailable()
		{
			ILoadRoboPassSeasonConfigRequest loadRoboPassSeasonConfigReq = serviceRequestFactory.Create<ILoadRoboPassSeasonConfigRequest>();
			loadRoboPassSeasonConfigReq.ClearCache();
			TaskService<RoboPassSeasonData> loadRoboPassSeasonConfigTS = loadRoboPassSeasonConfigReq.AsTask();
			yield return loadRoboPassSeasonConfigTS;
			if (!loadRoboPassSeasonConfigTS.succeeded)
			{
				Console.LogError("Failed to get RoboPass season config data: " + loadRoboPassSeasonConfigTS.behaviour.errorBody);
				yield return false;
				yield break;
			}
			RoboPassSeasonData roboPassSeasonData = loadRoboPassSeasonConfigTS.result;
			if (roboPassSeasonData == null)
			{
				yield return false;
			}
			else
			{
				yield return true;
			}
		}

		private IEnumerator DetermineAlreadyOwnedStatus()
		{
			RealMoneyStoreItemBundle dataItem = _dataSource.GetDataItem(_slotId, _slotType);
			int itemsOwnedCount = 0;
			foreach (RealMoneyStoreItem item2 in dataItem.Items)
			{
				RealMoneyStoreItem item = item2;
				if (item.StoreItemType == RealMoneyStoreItemType.RoboPass)
				{
					IEnumerator result = IsRoboPassOwned();
					yield return result;
					if ((bool)result.Current)
					{
						itemsOwnedCount++;
					}
				}
				else if (item.StoreItemType == RealMoneyStoreItemType.PremiumForLife)
				{
					if (premiumMembership.hasPremiumForLife)
					{
						itemsOwnedCount++;
					}
				}
				else if (item.StoreItemType == RealMoneyStoreItemType.Premium && premiumMembership.hasPremiumForLife)
				{
					itemsOwnedCount++;
				}
			}
			if (itemsOwnedCount == dataItem.Items.Count)
			{
				_view.SetAlreadyOwned(status: true);
				_view.ResetBannerOverlays();
			}
			else
			{
				_view.SetAlreadyOwned(status: false);
			}
		}

		private IEnumerator IsRoboPassOwned()
		{
			ILoadPlayerRoboPassSeasonRequest loadPlayerRoboPassSeasonReq = serviceRequestFactory.Create<ILoadPlayerRoboPassSeasonRequest>();
			loadPlayerRoboPassSeasonReq.ClearCache();
			TaskService<PlayerRoboPassSeasonData> loadPlayerRoboPassSeasonTS = loadPlayerRoboPassSeasonReq.AsTask();
			yield return loadPlayerRoboPassSeasonTS;
			if (!loadPlayerRoboPassSeasonTS.succeeded)
			{
				Console.LogError("Failed to get RoboPass player season data: " + loadPlayerRoboPassSeasonTS.behaviour.errorBody);
				yield return true;
				yield break;
			}
			PlayerRoboPassSeasonData playerRoboPassSeasonData = loadPlayerRoboPassSeasonTS.result;
			if (playerRoboPassSeasonData == null)
			{
				yield return false;
			}
			else
			{
				yield return playerRoboPassSeasonData.hasDeluxe;
			}
		}
	}
}
