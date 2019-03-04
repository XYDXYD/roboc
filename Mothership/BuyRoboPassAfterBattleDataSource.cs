using System;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class BuyRoboPassAfterBattleDataSource : IRealMoneyStoreItemDataSource
	{
		private readonly LoadingIconPresenter _loadingIconPresenter;

		private readonly List<RealMoneyStoreItemBundle> _roboPassBundles = new List<RealMoneyStoreItemBundle>();

		public event Action<bool> OnDataChanged;

		public BuyRoboPassAfterBattleDataSource(LoadingIconPresenter loadingIconPresenter)
		{
			_loadingIconPresenter = loadingIconPresenter;
		}

		public int GetDataItemsCount(RealMoneyStoreSlotDisplayType slotType)
		{
			if (_roboPassBundles == null)
			{
				return 0;
			}
			return _roboPassBundles.Count;
		}

		public RealMoneyStoreItemBundle GetDataItem(int index, RealMoneyStoreSlotDisplayType slotType)
		{
			if (index < _roboPassBundles.Count)
			{
				return _roboPassBundles[index];
			}
			RemoteLogger.Error("Attempting to fetch RealMoneyStoreItemBundle data with an out of bound index", $"Index = {index} and bundle count = {_roboPassBundles.Count}", null);
			return null;
		}

		public IEnumerator LoadData()
		{
			_loadingIconPresenter.NotifyLoading("BuyRoboPassAfterBattle");
			GetStoreItemsRequest request = new GetStoreItemsRequest();
			yield return request.Execute();
			if (request.error != null)
			{
				GenericErrorData error = new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strRobocloudError"), StringTableBase<StringTable>.Instance.GetString("strErrorUnableFetchShopProducts"));
				ErrorWindow.ShowErrorWindow(error);
				yield break;
			}
			_roboPassBundles.Clear();
			for (int i = 0; i < request.result.Count; i++)
			{
				RealMoneyStoreItemBundle realMoneyStoreItemBundle = request.result[i];
				if (IsBundleOnlyRoboPass(realMoneyStoreItemBundle))
				{
					_roboPassBundles.Add(realMoneyStoreItemBundle);
				}
			}
			_loadingIconPresenter.NotifyLoadingDone("BuyRoboPassAfterBattle");
			SafeEvent.SafeRaise<bool>(this.OnDataChanged, true);
		}

		private bool IsBundleOnlyRoboPass(RealMoneyStoreItemBundle bundleData)
		{
			foreach (RealMoneyStoreItem item in bundleData.Items)
			{
				RealMoneyStoreItem current = item;
				if (current.StoreItemType != 0)
				{
					return false;
				}
			}
			return true;
		}
	}
}
