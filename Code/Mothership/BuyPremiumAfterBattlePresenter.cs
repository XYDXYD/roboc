using Game.ECS.GUI.Components;
using Services.Analytics;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership
{
	internal class BuyPremiumAfterBattlePresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private BuyPremiumAfterBattleScreen _view;

		private RealMoneyStoreItemBundle _itemSelected;

		private RealMoneyStoreItemInfoView _itemInfoView;

		private IRealMoneyStoreItemDataSource _dataSource;

		private readonly PurchaseRequestFactory _purchaseRequestFactory;

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

		public BuyPremiumAfterBattlePresenter(PurchaseRequestFactory purchaseRequestFactory)
		{
			_purchaseRequestFactory = purchaseRequestFactory;
		}

		void IInitialize.OnDependenciesInjected()
		{
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
		}

		public bool IsActive()
		{
			return _view != null && _view.get_gameObject().get_activeSelf();
		}

		public void AddInfoViewButtonListeners(IRealMoneyStoreInfoViewButtonComponents infoViewButtons)
		{
			infoViewButtons.buyButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleBuyButtonPressed);
			infoViewButtons.goBackButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleCloseInfoOverlayButtonPressed);
		}

		private void HandleCloseInfoOverlayButtonPressed(int buttonId, bool clicked)
		{
			if (clicked)
			{
				_itemInfoView.Hide();
				PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "PurchaseCancelled", _itemSelected.ItemSku, startsNewChain: false);
			}
		}

		private void HandleBuyButtonPressed(int buttonId, bool clicked)
		{
			if (clicked)
			{
				_itemInfoView.Hide();
				PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "PurchaseConfirmed", _itemSelected.ItemSku, startsNewChain: false);
				_purchaseRequestFactory.CreateEntity(_itemSelected);
			}
		}

		public void AddBackButtonListener(IBuyPremiumAfterBattleButtonComponents viewButtons)
		{
			viewButtons.goBackButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleGoBackClicked);
		}

		private void HandleGoBackClicked(int viewId, bool clicked)
		{
			if (clicked)
			{
				_itemInfoView.Hide();
				guiInputController.ForceCloseJustThisScreen(GuiScreens.BuyPremiumAfterBattle);
			}
		}

		public void SetItemInfoView(RealMoneyStoreItemInfoView infoView)
		{
			_itemInfoView = infoView;
		}

		public void SetDataSource(IRealMoneyStoreItemDataSource dataSource)
		{
			_dataSource = dataSource;
		}

		public void AddCardViewButtonListener(IButtonComponent cardViewButton)
		{
			cardViewButton.buttonPressed.NotifyOnValueSet((Action<int, bool>)HandleBuyPremiumItemClicked);
		}

		public void HandleBuyPremiumItemClicked(int slotId, bool clicked)
		{
			RealMoneyStoreItemBundle realMoneyStoreItemBundle = _itemSelected = _dataSource.GetDataItem(slotId, RealMoneyStoreSlotDisplayType.PremiumRow);
			string key = "strRealMoneyStoreName_" + realMoneyStoreItemBundle.ItemSku;
			string @string = StringTableBase<StringTable>.Instance.GetString(key);
			string key2 = "strRealMoneyStoreDesc_" + realMoneyStoreItemBundle.ItemSku;
			string string2 = StringTableBase<StringTable>.Instance.GetString(key2);
			string currencyString = realMoneyStoreItemBundle.currencyString;
			_itemInfoView.SetItemInfo(@string, string2, currencyString);
			_itemInfoView.SetItemIcon(RealMoneyStoreSlotDisplayType.PremiumRow);
			bool isLifeTimePremium = false;
			int numberOfDays = 0;
			foreach (RealMoneyStoreItem item in realMoneyStoreItemBundle.Items)
			{
				RealMoneyStoreItem current = item;
				if (current.StoreItemType == RealMoneyStoreItemType.PremiumForLife)
				{
					isLifeTimePremium = true;
				}
				if (current.StoreItemType == RealMoneyStoreItemType.Premium)
				{
					isLifeTimePremium = false;
					numberOfDays = current.CountOfItems;
				}
			}
			_itemInfoView.ConfigurePremiumDetails(isLifeTimePremium, numberOfDays);
			_itemInfoView.Show();
		}

		public void SetView(BuyPremiumAfterBattleScreen view)
		{
			_view = view;
			_view.isShown.NotifyOnValueSet((Action<int, bool>)delegate(int id, bool shown)
			{
				if (shown)
				{
					RefreshData();
				}
				else
				{
					_itemInfoView.Hide();
				}
			});
		}

		private void RefreshData()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadAndReposition);
		}

		private IEnumerator LoadAndReposition()
		{
			yield return _dataSource.LoadData();
			_view.ContainerPremiumItems.set_repositionNow(true);
		}
	}
}
