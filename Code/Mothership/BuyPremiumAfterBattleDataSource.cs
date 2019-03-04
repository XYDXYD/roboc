using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class BuyPremiumAfterBattleDataSource : IRealMoneyStoreItemDataSource
	{
		private List<RealMoneyStoreItemBundle> _premiumBundles = new List<RealMoneyStoreItemBundle>();

		private readonly LoadingIconPresenter _loadingIcon;

		public event Action<bool> OnDataChanged;

		public BuyPremiumAfterBattleDataSource(LoadingIconPresenter loadingIcon)
		{
			_loadingIcon = loadingIcon;
		}

		public int GetDataItemsCount(RealMoneyStoreSlotDisplayType slotType)
		{
			if (_premiumBundles == null)
			{
				return 0;
			}
			return _premiumBundles.Count;
		}

		public RealMoneyStoreItemBundle GetDataItem(int index, RealMoneyStoreSlotDisplayType slotType)
		{
			return _premiumBundles[index];
		}

		public IEnumerator LoadData()
		{
			_loadingIcon.NotifyLoading("BuyPremiumAfterBattle");
			GetStoreItemsRequest request = new GetStoreItemsRequest();
			yield return request.Execute();
			if (request.error != null)
			{
				GenericErrorData error = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strRobocloudError"), StringTableBase<StringTable>.Instance.GetString("strErrorUnableFetchShopProducts"));
				ErrorWindow.ShowErrorWindow(error);
				yield break;
			}
			_premiumBundles = new List<RealMoneyStoreItemBundle>();
			foreach (RealMoneyStoreItemBundle item in request.result)
			{
				if (IsBundleOnlyPremium(item))
				{
					_premiumBundles.Add(item);
				}
			}
			_loadingIcon.NotifyLoadingDone("BuyPremiumAfterBattle");
			SafeEvent.SafeRaise<bool>(this.OnDataChanged, true);
		}

		private bool IsBundleOnlyPremium(RealMoneyStoreItemBundle bundleData)
		{
			foreach (RealMoneyStoreItem item in bundleData.Items)
			{
				RealMoneyStoreItem current = item;
				if (current.StoreItemType != RealMoneyStoreItemType.Premium && current.StoreItemType != RealMoneyStoreItemType.PremiumForLife)
				{
					return false;
				}
			}
			return true;
		}
	}
}
