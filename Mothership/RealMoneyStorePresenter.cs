using Game.ECS.GUI.Components;
using Services.Analytics;
using Svelto.Context;
using Svelto.IoC;
using System;
using System.Collections;

namespace Mothership
{
	internal class RealMoneyStorePresenter : IInitialize, IWaitForFrameworkDestruction
	{
		private RealMoneyStoreItemBundle _itemSelected;

		private RealMoneyStoreScreen _view;

		private RealMoneyStoreItemInfoView _itemInfoView;

		private IRealMoneyStoreItemDataSource _dataSource;

		private IRealMoneyStorePossibleRoboPassItemsDataSource _possibleItemsDataSource;

		private readonly PurchaseRequestFactory _purchaseRequestFactory;

		private readonly IAnalyticsRequestFactory _analyticsRequestFactory;

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			get;
			private set;
		}

		public RealMoneyStorePresenter(PurchaseRequestFactory purchaseRequestFactory, IAnalyticsRequestFactory analyticsRequestFactory)
		{
			_purchaseRequestFactory = purchaseRequestFactory;
			_analyticsRequestFactory = analyticsRequestFactory;
		}

		void IInitialize.OnDependenciesInjected()
		{
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
		}

		public void AddInfoViewButtonListeners(IRealMoneyStoreInfoViewButtonComponents infoViewButtons)
		{
			infoViewButtons.buyButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleBuyButtonPressed);
			infoViewButtons.goBackButtonPressed.NotifyOnValueSet((Action<int, bool>)HandleGoBackButtonPressed);
		}

		public void AddCardViewButtonListener(IButtonComponent cardViewButton, RealMoneyStoreSlotDisplayType slotType)
		{
			switch (slotType)
			{
			case RealMoneyStoreSlotDisplayType.CosmeticCreditsRow:
				cardViewButton.buttonPressed.NotifyOnValueSet((Action<int, bool>)HandleCosmeticCreditsItemPicked);
				break;
			case RealMoneyStoreSlotDisplayType.PremiumRow:
				cardViewButton.buttonPressed.NotifyOnValueSet((Action<int, bool>)HandlePremiumItemPicked);
				break;
			case RealMoneyStoreSlotDisplayType.Robopass:
				cardViewButton.buttonPressed.NotifyOnValueSet((Action<int, bool>)HandleRoboPassItemPicked);
				break;
			}
		}

		private void HandleCosmeticCreditsItemPicked(int slotId, bool clicked)
		{
			if (clicked)
			{
				HandleCardViewButtonClick(slotId, RealMoneyStoreSlotDisplayType.CosmeticCreditsRow);
			}
		}

		private void HandlePremiumItemPicked(int slotId, bool clicked)
		{
			if (clicked)
			{
				HandleCardViewButtonClick(slotId, RealMoneyStoreSlotDisplayType.PremiumRow);
			}
		}

		private void HandleRoboPassItemPicked(int slotId, bool clicked)
		{
			if (clicked)
			{
				HandleCardViewButtonClick(slotId, RealMoneyStoreSlotDisplayType.Robopass);
			}
		}

		private void HandleCardViewButtonClick(int slotId, RealMoneyStoreSlotDisplayType slotType)
		{
			RealMoneyStoreItemBundle realMoneyStoreItemBundle = _itemSelected = _dataSource.GetDataItem(slotId, slotType);
			string key = "strRealMoneyStoreName_" + realMoneyStoreItemBundle.ItemSku;
			string @string = StringTableBase<StringTable>.Instance.GetString(key);
			string key2 = "strRealMoneyStoreDesc_" + realMoneyStoreItemBundle.ItemSku;
			string string2 = StringTableBase<StringTable>.Instance.GetString(key2);
			string currencyString = realMoneyStoreItemBundle.currencyString;
			_itemInfoView.SetItemInfo(@string, string2, currencyString);
			_itemInfoView.SetItemIcon(slotType);
			if (slotType == RealMoneyStoreSlotDisplayType.PremiumRow)
			{
				bool isLifeTimePremium = false;
				int numberOfDays = 0;
				foreach (RealMoneyStoreItem item in realMoneyStoreItemBundle.Items)
				{
					RealMoneyStoreItem current = item;
					if (current.StoreItemType == RealMoneyStoreItemType.PremiumForLife)
					{
						isLifeTimePremium = true;
					}
					else if (current.StoreItemType == RealMoneyStoreItemType.Premium)
					{
						numberOfDays = current.CountOfItems;
					}
				}
				_itemInfoView.ConfigurePremiumDetails(isLifeTimePremium, numberOfDays);
			}
			if (slotType == RealMoneyStoreSlotDisplayType.CosmeticCreditsRow)
			{
				_itemInfoView.ConfigureCosmeticCreditDetails(realMoneyStoreItemBundle.ItemSku);
			}
			PurchaseFunnelHelper.SendEvent(_analyticsRequestFactory, "SKUSelected", realMoneyStoreItemBundle.ItemSku, startsNewChain: false);
			_itemInfoView.Show();
		}

		private void HandleBuyButtonPressed(int buttonId, bool clicked)
		{
			if (clicked)
			{
				_itemInfoView.Hide();
				PurchaseFunnelHelper.SendEvent(_analyticsRequestFactory, "PurchaseSelected", _itemSelected.ItemSku, startsNewChain: false);
				_purchaseRequestFactory.CreateEntity(_itemSelected);
			}
		}

		private void HandleGoBackButtonPressed(int buttonId, bool clicked)
		{
			if (clicked)
			{
				_itemInfoView.Hide();
				PurchaseFunnelHelper.SendEvent(_analyticsRequestFactory, "PurchaseCancelled", _itemSelected.ItemSku, startsNewChain: false);
			}
		}

		public void SetDataSources(IRealMoneyStoreItemDataSource dataSource, IRealMoneyStorePossibleRoboPassItemsDataSource possibleItemsDataSource)
		{
			_dataSource = dataSource;
			_possibleItemsDataSource = possibleItemsDataSource;
		}

		public void SetView(RealMoneyStoreScreen view)
		{
			_view = view;
			_view.isShown.NotifyOnValueSet((Action<int, bool>)delegate(int id, bool shown)
			{
				if (!shown)
				{
					_itemInfoView.Hide();
				}
			});
		}

		public void SetItemInfoView(RealMoneyStoreItemInfoView infoView)
		{
			_itemInfoView = infoView;
		}

		public IEnumerator LoadGUIData()
		{
			yield return _dataSource.LoadData();
			yield return _possibleItemsDataSource.LoadData();
		}

		internal void Show()
		{
			guiInputController.ShowScreen(GuiScreens.RealMoneyStoreScreen);
		}
	}
}
