using Services.Analytics;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class BuyRoboPassAfterBattlePresenter
	{
		private readonly PurchaseRequestFactory _purchaseRequestFactory;

		private RealMoneyStoreItemInfoView _view;

		private IRealMoneyStoreItemDataSource _dataSource;

		[Inject]
		internal IGUIInputController guiInputController
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

		public BuyRoboPassAfterBattlePresenter(PurchaseRequestFactory purchaseRequestFactory)
		{
			_purchaseRequestFactory = purchaseRequestFactory;
		}

		public void SetView(RealMoneyStoreItemInfoView view)
		{
			_view = view;
			_view.isShown.NotifyOnValueSet((Action<int, bool>)HandleScreenShown);
		}

		public void AddButtonListener(IRealMoneyStoreInfoViewButtonComponents viewButtons)
		{
			viewButtons.goBackButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleGoBackClicked);
			viewButtons.buyButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleBuyNowClicked);
		}

		public void SetDataSource(IRealMoneyStoreItemDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		private void HandleScreenShown(int id, bool shown)
		{
			if (shown)
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)RefreshDataAndShow);
			}
			else
			{
				_view.Hide();
			}
		}

		private IEnumerator RefreshDataAndShow()
		{
			yield return _dataSource.LoadData();
			Show();
		}

		private void Show()
		{
			RealMoneyStoreItemBundle dataItem = _dataSource.GetDataItem(0, RealMoneyStoreSlotDisplayType.Robopass);
			if (dataItem == null)
			{
				ShowDataFetchErrorWindow();
				return;
			}
			string key = "strRealMoneyStoreName_" + dataItem.ItemSku;
			string @string = StringTableBase<StringTable>.Instance.GetString(key);
			string key2 = "strRealMoneyStoreDesc_" + dataItem.ItemSku;
			string string2 = StringTableBase<StringTable>.Instance.GetString(key2);
			string currencyString = dataItem.currencyString;
			_view.SetItemInfo(@string, string2, currencyString);
			_view.SetItemIcon(RealMoneyStoreSlotDisplayType.Robopass);
			_view.RoboPassPossibleItemsContainer.set_repositionNow(true);
			_view.Show();
		}

		private void HandleGoBackClicked(int viewId, bool clicked)
		{
			if (clicked)
			{
				guiInputController.ForceCloseJustThisScreen(GuiScreens.BuyRoboPassAfterBattle);
				RealMoneyStoreItemBundle dataItem = _dataSource.GetDataItem(0, RealMoneyStoreSlotDisplayType.Robopass);
				if (dataItem != null)
				{
					PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "PurchaseCancelled", dataItem.ItemSku, startsNewChain: false);
				}
			}
		}

		private void HandleBuyNowClicked(int viewId, bool clicked)
		{
			if (clicked)
			{
				RealMoneyStoreItemBundle dataItem = _dataSource.GetDataItem(0, RealMoneyStoreSlotDisplayType.Robopass);
				if (dataItem != null)
				{
					PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "PurchaseConfirmed", dataItem.ItemSku, startsNewChain: false);
					_purchaseRequestFactory.CreateEntity(dataItem);
				}
				else
				{
					ShowDataFetchErrorWindow();
				}
			}
		}

		private void ShowDataFetchErrorWindow()
		{
			ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreFetchDataErrorTitle"), StringTableBase<StringTable>.Instance.GetString("strRealMoneyStoreFetchDataErrorBody"), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
			{
				guiInputController.ForceCloseJustThisScreen(GuiScreens.BuyRoboPassAfterBattle);
			}));
		}
	}
}
