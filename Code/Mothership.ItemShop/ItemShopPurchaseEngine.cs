using Services.Analytics;
using Services.Requests.Interfaces;
using Services.Web.Photon;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Utility;

namespace Mothership.ItemShop
{
	internal class ItemShopPurchaseEngine : MultiEntityViewsEngine<ItemShopDisplayEntityView, ItemShopPopUpEntityView, ItemShopBundleEntityView>, IInitialize, IQueryingEntityViewEngine, IEngine
	{
		private struct CurrencyTypeData
		{
			public readonly string CurrencyTypeStr;

			public readonly string PurchaseCurrencyTypeStr;

			public readonly bool CanPurchaseCurrencyType;

			public CurrencyTypeData(string currencyTypeStr, string purchaseCurrencyTypeStr, bool canPurchaseCurrencyType)
			{
				CurrencyTypeStr = currencyTypeStr;
				PurchaseCurrencyTypeStr = purchaseCurrencyTypeStr;
				CanPurchaseCurrencyType = canPurchaseCurrencyType;
			}
		}

		private bool _isShopAvailable = true;

		private StringBuilder _sb = new StringBuilder();

		[Inject]
		internal ICurrenciesTracker currencyTracker
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal LoadingIconPresenter loadingIconPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal ICubeInventory cubeInventory
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

		[Inject]
		internal LoadingIconPresenter loadingPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			get;
			private set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		public void OnDependenciesInjected()
		{
			ILoadPlatformConfigurationRequest request = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> taskService = request.AsTask();
			taskService.Execute();
			_isShopAvailable = taskService.result.MainShopButtonAvailable;
		}

		protected override void Add(ItemShopDisplayEntityView entityView)
		{
			entityView.showComponent.isShown.NotifyOnValueSet((Action<int, bool>)ClosePopupsOnDisplayToggled);
		}

		protected override void Remove(ItemShopDisplayEntityView entityView)
		{
			entityView.showComponent.isShown.StopNotify((Action<int, bool>)ClosePopupsOnDisplayToggled);
		}

		protected override void Add(ItemShopPopUpEntityView popupEntityView)
		{
			popupEntityView.dialogChoiceComponent.validatePressed.NotifyOnValueSet((Action<int, bool>)ConfirmButtonClicked);
			popupEntityView.dialogChoiceComponent.cancelPressed.NotifyOnValueSet((Action<int, bool>)CancelActivePopUp);
		}

		protected override void Remove(ItemShopPopUpEntityView popupEntityView)
		{
			popupEntityView.dialogChoiceComponent.validatePressed.StopNotify((Action<int, bool>)ConfirmButtonClicked);
			popupEntityView.dialogChoiceComponent.cancelPressed.StopNotify((Action<int, bool>)CancelActivePopUp);
		}

		protected override void Add(ItemShopBundleEntityView productEntityView)
		{
			productEntityView.buttonComponent.buttonPressed.NotifyOnValueSet((Action<int, bool>)TryToBuyBundle);
		}

		protected override void Remove(ItemShopBundleEntityView productEntityView)
		{
			productEntityView.buttonComponent.buttonPressed.StopNotify((Action<int, bool>)TryToBuyBundle);
		}

		private void TryToBuyBundle(int bundleEntityId, bool selected)
		{
			if (selected)
			{
				ItemShopBundleEntityView itemShopBundleEntityView = entityViewsDB.QueryEntityView<ItemShopBundleEntityView>(bundleEntityId);
				TaskRunner.get_Instance().Run(ValidatePlayerCanPurchase(itemShopBundleEntityView.bundleComponent.bundle));
			}
		}

		private IEnumerator ValidatePlayerCanPurchase(ItemShopBundle selectedBundle)
		{
			if (!selectedBundle.OwnsRequiredCube)
			{
				ShowPopUp(ItemShopPopUpType.UNABLE_TO_PURCHASE, selectedBundle);
				yield break;
			}
			Wallet? wallet = null;
			currencyTracker.RetrieveCurrentWallet(delegate(Wallet response)
			{
				wallet = response;
			});
			while (!wallet.HasValue)
			{
				yield return null;
			}
			long balance = (selectedBundle.CurrencyType != 0) ? wallet.Value.CosmeticCreditsBalance : wallet.Value.RobitsBalance;
			if (balance >= selectedBundle.Price)
			{
				ShowPopUp(ItemShopPopUpType.CONFIRM_PURCHASE, selectedBundle);
			}
			else
			{
				ShowPopUp(ItemShopPopUpType.INSUFFICIENT_FUNDS, selectedBundle);
			}
		}

		private ItemShopPopUpEntityView GetPopup()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return entityViewsDB.QueryEntityViews<ItemShopPopUpEntityView>().get_Item(0);
		}

		private void ShowPopUp(ItemShopPopUpType popupType, ItemShopBundle bundle)
		{
			ItemShopPopUpEntityView popup = GetPopup();
			IItemShopPopUpComponent itemShopPopUpComponent = popup.itemShopPopUpComponent;
			itemShopPopUpComponent.popupType = popupType;
			switch (popupType)
			{
			case ItemShopPopUpType.INSUFFICIENT_FUNDS:
				HandleInsufficientFunds(itemShopPopUpComponent, bundle);
				break;
			case ItemShopPopUpType.CONFIRM_PURCHASE:
				HandleConfirmPurchase(itemShopPopUpComponent, bundle);
				break;
			case ItemShopPopUpType.PURCHASE_SUCCESS:
				HandlePurchaseSuccess(itemShopPopUpComponent, bundle);
				break;
			case ItemShopPopUpType.UNABLE_TO_PURCHASE:
				HandleUnableToPurchase(itemShopPopUpComponent, bundle);
				break;
			case ItemShopPopUpType.SALE_ENDED_CONFIRM:
				HandleSaleEndedConfirmPurchase(itemShopPopUpComponent, bundle);
				bundle = new ItemShopBundle(bundle.SKU, bundle.BundleNameStrKey, bundle.SpriteName, bundle.IsSpriteFullSize, bundle.Category, bundle.CurrencyType, bundle.Price, bundle.DiscountPrice, bundle.Recurrence, bundle.OwnsRequiredCube, discounted: false, bundle.LimitedEdition);
				break;
			}
			popup.bundleComponent.bundle = bundle;
			popup.showComponent.isShown.set_value(true);
		}

		private void HandleInsufficientFunds(IItemShopPopUpComponent popUpComponent, ItemShopBundle bundle)
		{
			CurrencyTypeData currencyTypeData = GetCurrencyTypeData(bundle.CurrencyType);
			bool flag = _isShopAvailable && currencyTypeData.CanPurchaseCurrencyType;
			popUpComponent.titleLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strInsufficientFunds"));
			popUpComponent.infoLabel.set_text(StringTableBase<StringTable>.Instance.GetReplaceString("strInsufficientFundsDesc", "[CURRENCY NAME]", currencyTypeData.CurrencyTypeStr));
			popUpComponent.singleButtonContainer.SetActive(!flag);
			popUpComponent.doubleButtonsContainer.SetActive(flag);
			if (flag)
			{
				popUpComponent.leftButtLabel.set_text(currencyTypeData.PurchaseCurrencyTypeStr);
				popUpComponent.rightButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strDismiss"));
			}
			else
			{
				popUpComponent.centerButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strDismiss"));
			}
		}

		private void HandleConfirmPurchase(IItemShopPopUpComponent popUpComponent, ItemShopBundle bundle)
		{
			CurrencyTypeData currencyTypeData = GetCurrencyTypeData(bundle.CurrencyType);
			popUpComponent.titleLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirmPurchase"));
			_sb.Length = 0;
			_sb.Append(StringTableBase<StringTable>.Instance.GetString("strStoreConfirmPurchaseDesc"));
			_sb.Replace("[PRICE]", bundle.GetFinalPrice().ToString("N0"));
			_sb.Replace("[CURRENCY TYPE]", currencyTypeData.CurrencyTypeStr);
			_sb.Replace("[BUNDLE NAME]", StringTableBase<StringTable>.Instance.GetString(bundle.BundleNameStrKey));
			popUpComponent.infoLabel.set_text(_sb.ToString());
			popUpComponent.singleButtonContainer.SetActive(false);
			popUpComponent.doubleButtonsContainer.SetActive(true);
			popUpComponent.leftButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirm"));
			popUpComponent.rightButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCancel"));
		}

		private void HandleSaleEndedConfirmPurchase(IItemShopPopUpComponent popUpComponent, ItemShopBundle bundle)
		{
			CurrencyTypeData currencyTypeData = GetCurrencyTypeData(bundle.CurrencyType);
			popUpComponent.titleLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strSaleEnded"));
			_sb.Length = 0;
			_sb.Append(StringTableBase<StringTable>.Instance.GetString("strStoreSaleEndedConfirmPurchaseDesc"));
			_sb.Replace("[PRICE]", bundle.Price.ToString("N0"));
			_sb.Replace("[CURRENCY TYPE]", currencyTypeData.CurrencyTypeStr);
			_sb.Replace("[BUNDLE NAME]", StringTableBase<StringTable>.Instance.GetString(bundle.BundleNameStrKey));
			popUpComponent.infoLabel.set_text(_sb.ToString());
			popUpComponent.singleButtonContainer.SetActive(false);
			popUpComponent.doubleButtonsContainer.SetActive(true);
			popUpComponent.leftButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strConfirm"));
			popUpComponent.rightButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCancel"));
		}

		private void HandlePurchaseSuccess(IItemShopPopUpComponent popUpComponent, ItemShopBundle bundle)
		{
			popUpComponent.titleLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strItemShopPurchaseSuccessTitle"));
			popUpComponent.infoLabel.set_text(StringTableBase<StringTable>.Instance.GetReplaceString("strItemShopPurchaseSuccessInfo", "[BUNDLE]", StringTableBase<StringTable>.Instance.GetString(bundle.BundleNameStrKey)));
			popUpComponent.singleButtonContainer.SetActive(true);
			popUpComponent.doubleButtonsContainer.SetActive(false);
			popUpComponent.centerButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strOK"));
		}

		private void HandleUnableToPurchase(IItemShopPopUpComponent popUpComponent, ItemShopBundle bundle)
		{
			popUpComponent.titleLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strItemShopPurchaseErrorTitle"));
			if (!bundle.OwnsRequiredCube)
			{
				popUpComponent.infoLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strItemShopLockedError"));
			}
			popUpComponent.singleButtonContainer.SetActive(true);
			popUpComponent.doubleButtonsContainer.SetActive(false);
			popUpComponent.centerButtLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strOK"));
		}

		private CurrencyTypeData GetCurrencyTypeData(CurrencyType currencyType)
		{
			switch (currencyType)
			{
			case CurrencyType.CosmeticCredits:
				return new CurrencyTypeData(StringTableBase<StringTable>.Instance.GetString("strCosmeticCredits"), StringTableBase<StringTable>.Instance.GetString("strPurchaseCC"), canPurchaseCurrencyType: true);
			case CurrencyType.Robits:
				return new CurrencyTypeData(StringTableBase<StringTable>.Instance.GetString("strRobits"), null, canPurchaseCurrencyType: false);
			default:
				throw new Exception("Attempted to retrieve CurrencyTypeData for an unmapped currency type: " + currencyType);
			}
		}

		private void ConfirmButtonClicked(int entityID, bool clicked)
		{
			if (clicked)
			{
				ItemShopPopUpEntityView itemShopPopUpEntityView = entityViewsDB.QueryEntityView<ItemShopPopUpEntityView>(entityID);
				itemShopPopUpEntityView.showComponent.isShown.set_value(false);
				IItemShopPopUpComponent itemShopPopUpComponent = itemShopPopUpEntityView.itemShopPopUpComponent;
				switch (itemShopPopUpComponent.popupType)
				{
				case ItemShopPopUpType.INSUFFICIENT_FUNDS:
					ShowStore(itemShopPopUpEntityView.bundleComponent.bundle);
					break;
				case ItemShopPopUpType.CONFIRM_PURCHASE:
				case ItemShopPopUpType.SALE_ENDED_CONFIRM:
					TaskRunner.get_Instance().Run(StartPurchaseBundleRequest(itemShopPopUpEntityView.bundleComponent.bundle));
					break;
				}
				itemShopPopUpEntityView.bundleComponent.bundle = null;
			}
		}

		private void ShowStore(ItemShopBundle selectedBundle)
		{
			CurrencyType currencyType = selectedBundle.CurrencyType;
			if (currencyType == CurrencyType.CosmeticCredits)
			{
				PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "StoreEntered", "ItemShop", startsNewChain: true);
				guiInputController.ShowScreen(GuiScreens.RealMoneyStoreScreen);
				return;
			}
			throw new Exception("Attempted to show store with an unsupported currency type");
		}

		private IEnumerator StartPurchaseBundleRequest(ItemShopBundle selectedBundle)
		{
			loadingIconPresenter.NotifyLoading("PurchaseItemShop");
			IBuyItemShopBundleRequest request = serviceFactory.Create<IBuyItemShopBundleRequest, ItemShopBundle>(selectedBundle);
			TaskService<string[]> task = new TaskService<string[]>(request);
			yield return task;
			if (task.succeeded)
			{
				string[] newCubes = task.result;
				for (int i = 0; i < newCubes.Length; i++)
				{
					uint item = uint.Parse(newCubes[i], NumberStyles.AllowHexSpecifier);
					ISetNewInventoryCubesRequest setNewInventoryCubesRequest = serviceFactory.Create<ISetNewInventoryCubesRequest>();
					setNewInventoryCubesRequest.Inject(new HashSet<uint>
					{
						item
					});
					setNewInventoryCubesRequest.Execute();
				}
				yield return currencyTracker.RefreshUserWalletEnumerator();
				yield return cubeInventory.RefreshAndWait();
				ILoadAllCustomisationInfoRequest loadallrequest = serviceFactory.Create<ILoadAllCustomisationInfoRequest>();
				loadallrequest.ClearCache();
				yield return new TaskService<AllCustomisationsResponse>(loadallrequest);
				FasterListEnumerator<ItemShopDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ItemShopDisplayEntityView current = enumerator.get_Current();
						current.itemShopDisplayComponent.lastRefreshReason = RefreshReason.ItemBought;
						current.itemShopDisplayComponent.refresh.set_value(true);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				yield return HandleAnalyticsForItemBought(selectedBundle, newCubes);
				loadingIconPresenter.NotifyLoadingDone("PurchaseItemShop");
				ShowPopUp(ItemShopPopUpType.PURCHASE_SUCCESS, selectedBundle);
			}
			else
			{
				loadingIconPresenter.NotifyLoadingDone("PurchaseItemShop");
				switch (task.behaviour.errorCode)
				{
				case 147:
					ErrorWindow.ShowErrorWindow(new GenericErrorData(StringTableBase<StringTable>.Instance.GetString("strItemShopPurchaseErrorTitle"), StringTableBase<StringTable>.Instance.GetString("strItemShopPurchaseInfo"), StringTableBase<StringTable>.Instance.GetString("strOK"), delegate
					{
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0010: Unknown result type (might be due to invalid IL or missing references)
						//IL_0013: Unknown result type (might be due to invalid IL or missing references)
						//IL_0018: Unknown result type (might be due to invalid IL or missing references)
						FasterListEnumerator<ItemShopDisplayEntityView> enumerator2 = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								ItemShopDisplayEntityView current2 = enumerator2.get_Current();
								current2.itemShopDisplayComponent.lastRefreshReason = RefreshReason.ShopRefresh;
								current2.itemShopDisplayComponent.refresh.set_value(true);
							}
						}
						finally
						{
							((IDisposable)enumerator2).Dispose();
						}
					}));
					break;
				case 17:
					ShowPopUp(ItemShopPopUpType.INSUFFICIENT_FUNDS, selectedBundle);
					break;
				case 207:
					ShowPopUp(ItemShopPopUpType.SALE_ENDED_CONFIRM, selectedBundle);
					break;
				default:
					ErrorWindow.ShowServiceErrorWindow(task.behaviour);
					break;
				}
			}
		}

		private void CancelActivePopUp(int entityID, bool clicked)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (clicked)
			{
				ItemShopPopUpEntityView itemShopPopUpEntityView = entityViewsDB.QueryEntityView<ItemShopPopUpEntityView>(entityID);
				if (itemShopPopUpEntityView.itemShopPopUpComponent.popupType == ItemShopPopUpType.SALE_ENDED_CONFIRM)
				{
					FasterListEnumerator<ItemShopDisplayEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopDisplayEntityView>().GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							ItemShopDisplayEntityView current = enumerator.get_Current();
							current.itemShopDisplayComponent.lastRefreshReason = RefreshReason.ShopRefresh;
							current.itemShopDisplayComponent.refresh.set_value(true);
						}
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				itemShopPopUpEntityView.showComponent.isShown.set_value(false);
			}
		}

		private void ClosePopupsOnDisplayToggled(int displayEntityID, bool shown)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<ItemShopPopUpEntityView> enumerator = entityViewsDB.QueryEntityViews<ItemShopPopUpEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ItemShopPopUpEntityView current = enumerator.get_Current();
					current.showComponent.isShown.set_value(false);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private IEnumerator HandleAnalyticsForItemBought(ItemShopBundle itemShopBundle, string[] newCubes)
		{
			loadingPresenter.NotifyLoading("HandleAnalytics");
			string context = ItemShopAnalyticsUtility.GetContext(itemShopBundle.Recurrence);
			string type = ItemShopAnalyticsUtility.GetCategory(itemShopBundle.Category);
			if (context == null || type == null)
			{
				loadingPresenter.NotifyLoadingDone("HandleAnalytics");
				yield break;
			}
			LogItemBoughtDependency itemBoughtDependency = new LogItemBoughtDependency(itemShopBundle.BundleNameStrKey, type, itemShopBundle.CurrencyType.ToString(), itemShopBundle.Price, itemShopBundle.GetDiscountPercent(), context);
			TaskService logItemBoughtRequest = analyticsRequestFactory.Create<ILogItemBoughtRequest, LogItemBoughtDependency>(itemBoughtDependency).AsTask();
			yield return logItemBoughtRequest;
			if (!logItemBoughtRequest.succeeded)
			{
				Console.LogError("Log Item Bought Request failed. " + logItemBoughtRequest.behaviour.exceptionThrown);
			}
			long balance = 0L;
			currencyTracker.RetrieveCurrentWallet(delegate(Wallet wallet)
			{
				if (itemShopBundle.CurrencyType == CurrencyType.CosmeticCredits)
				{
					balance = wallet.CosmeticCreditsBalance;
				}
				else
				{
					balance = wallet.RobitsBalance;
				}
			});
			LogPlayerCurrencySpentDependency playerCurrencySpentDependency = new LogPlayerCurrencySpentDependency(itemShopBundle.CurrencyType.ToString(), itemShopBundle.Price, balance, "ItemShop", itemShopBundle.BundleNameStrKey);
			TaskService logPlayerCurrencySpentRequest = analyticsRequestFactory.Create<ILogPlayerCurrencySpentRequest, LogPlayerCurrencySpentDependency>(playerCurrencySpentDependency).AsTask();
			yield return logPlayerCurrencySpentRequest;
			if (!logPlayerCurrencySpentRequest.succeeded)
			{
				Console.LogError("Log Player Currency Spent Request failed. " + logPlayerCurrencySpentRequest.behaviour.exceptionThrown);
			}
			loadingPresenter.NotifyLoadingDone("HandleAnalytics");
		}
	}
}
