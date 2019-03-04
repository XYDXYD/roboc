using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class RealMoneyStoreDataSource : IRealMoneyStoreItemDataSource
	{
		private List<RealMoneyStoreItemBundle> _allStoreBundles = new List<RealMoneyStoreItemBundle>();

		private List<RealMoneyStoreItemBundle>[] _bundlesByCategory = new List<RealMoneyStoreItemBundle>[3];

		private readonly LoadingIconPresenter _loadingIcon;

		public event Action<bool> OnDataChanged;

		public RealMoneyStoreDataSource(LoadingIconPresenter loadingIcon)
		{
			_loadingIcon = loadingIcon;
		}

		public int GetDataItemsCount(RealMoneyStoreSlotDisplayType slotType)
		{
			if (_bundlesByCategory[(int)slotType] == null)
			{
				return 0;
			}
			return _bundlesByCategory[(int)slotType].Count;
		}

		public RealMoneyStoreItemBundle GetDataItem(int index, RealMoneyStoreSlotDisplayType slotType)
		{
			return _bundlesByCategory[(int)slotType][index];
		}

		public IEnumerator LoadData()
		{
			_loadingIcon.NotifyLoading("RealMoneyStore");
			GetStoreItemsRequest request = new GetStoreItemsRequest();
			yield return request.Execute();
			if (request.error != null)
			{
				GenericErrorData error = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strRobocloudError"), StringTableBase<StringTable>.Instance.GetString("strErrorUnableFetchShopProducts"));
				ErrorWindow.ShowErrorWindow(error);
				yield break;
			}
			_allStoreBundles = request.result;
			_bundlesByCategory = new List<RealMoneyStoreItemBundle>[3];
			for (int i = 0; i < 3; i++)
			{
				_bundlesByCategory[i] = new List<RealMoneyStoreItemBundle>();
			}
			foreach (RealMoneyStoreItemBundle allStoreBundle in _allStoreBundles)
			{
				foreach (RealMoneyStoreItem item in allStoreBundle.Items)
				{
					RealMoneyStoreItem realMoneyStoreItem = item;
				}
				RealMoneyStoreSlotDisplayType slotDisplayTypeForItemsInBundle = GetSlotDisplayTypeForItemsInBundle(allStoreBundle);
				_bundlesByCategory[(int)slotDisplayTypeForItemsInBundle].Add(allStoreBundle);
			}
			_loadingIcon.NotifyLoadingDone("RealMoneyStore");
			SafeEvent.SafeRaise<bool>(this.OnDataChanged, true);
		}

		private RealMoneyStoreSlotDisplayType GetSlotDisplayTypeForItemsInBundle(RealMoneyStoreItemBundle bundleData)
		{
			foreach (RealMoneyStoreItem item in bundleData.Items)
			{
				RealMoneyStoreItem current = item;
				if (current.StoreItemType == RealMoneyStoreItemType.Premium || current.StoreItemType == RealMoneyStoreItemType.PremiumForLife)
				{
					return RealMoneyStoreSlotDisplayType.PremiumRow;
				}
				if (current.StoreItemType == RealMoneyStoreItemType.RoboPass)
				{
					return RealMoneyStoreSlotDisplayType.Robopass;
				}
			}
			return RealMoneyStoreSlotDisplayType.CosmeticCreditsRow;
		}
	}
}
