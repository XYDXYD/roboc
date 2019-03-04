using Services.Analytics;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.Observer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace Mothership
{
	internal class ExecutePurchaseEngine : SingleEntityViewEngine<ExecutePurchaseEntityView>, IQueryingEntityViewEngine, IHandleEditingInput, IEngine, IInputComponent, IComponent
	{
		private readonly LoadingIconPresenter _loadingIconPresenter;

		private readonly IGUIInputController _guiInputController;

		private readonly IBrowser _browser;

		private readonly BrowserURLChangedObserver _browserURLChangedObserver;

		private readonly BrowserClosedObserver _browserClosedObserver;

		private readonly PurchaseRefresher _purchaseRefresher;

		private readonly IEntityFunctions _entityFunctions;

		private readonly PriceChangeDialogPresenter _priceChangeDialogPresenter;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public ExecutePurchaseEngine(LoadingIconPresenter loadingIconPresenter, IGUIInputController guiInputController, IBrowser browser, BrowserURLChangedObserver browserURLChangedObserver, BrowserClosedObserver browserClosedObserver, PurchaseRefresher purchaseRefresher, IAnalyticsRequestFactory analyticsRequestFactory, IEntityFunctions entityFunctions, PriceChangeDialogPresenter priceChangeDialogPresenter)
		{
			_loadingIconPresenter = loadingIconPresenter;
			_guiInputController = guiInputController;
			_browser = browser;
			_browserURLChangedObserver = browserURLChangedObserver;
			_browserClosedObserver = browserClosedObserver;
			_purchaseRefresher = purchaseRefresher;
			_entityFunctions = entityFunctions;
			_priceChangeDialogPresenter = priceChangeDialogPresenter;
		}

		public void Ready()
		{
		}

		protected override void Add(ExecutePurchaseEntityView entityView)
		{
			TaskRunner.get_Instance().Run(OpenBrowser(entityView));
		}

		protected override void Remove(ExecutePurchaseEntityView entityView)
		{
		}

		public void HandleEditingInput(InputEditingData data)
		{
			if (data[EditingInputAxis.QUIT] != 0f)
			{
				_browser.Hide();
			}
		}

		private unsafe IEnumerator OpenBrowser(ExecutePurchaseEntityView entityView)
		{
			_guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			string text = StringTableBase<StringTable>.Instance.GetString("strConnectingToShop");
			_loadingIconPresenter.NotifyLoading("XSollaShop", text);
			RealMoneyStoreItemBundle item3 = entityView.purchaseRequestComponent.item;
			PurchaseItemRequest request3 = new PurchaseItemRequest(item3);
			yield return request3.Execute();
			if (request3.error != null)
			{
				GenericErrorData error = new GenericErrorData("strRobocloudError", "strItemShopPurchaseErrorTitle");
				ErrorWindow.ShowErrorWindow(error);
				_entityFunctions.RemoveEntity<PurchaseRequestEntityDescriptor>(entityView.get_ID());
				yield break;
			}
			string url = request3.url;
			if (request3.newPrice != null)
			{
				Regex rgx = new Regex("[^0-9.-]");
				string str = rgx.Replace(request3.newPrice, string.Empty);
				float newPrice = Convert.ToSingle(str);
				_loadingIconPresenter.NotifyLoadingDone("XSollaShop");
				PriceChangeDialogTask priceChangeDialogTask = new PriceChangeDialogTask(_priceChangeDialogPresenter, request3.newPrice);
				yield return priceChangeDialogTask;
				if (!priceChangeDialogTask.succeeded)
				{
					_entityFunctions.RemoveEntity<PurchaseRequestEntityDescriptor>(entityView.get_ID());
					yield break;
				}
				RealMoneyStoreItemBundle item2 = entityView.purchaseRequestComponent.item;
				item2.priceForCheck = newPrice;
				PurchaseItemRequest request2 = new PurchaseItemRequest(item2);
				_loadingIconPresenter.NotifyLoading("XSollaShop");
				yield return request2.Execute();
				_loadingIconPresenter.NotifyLoadingDone("XSollaShop");
				if (request2.error != null)
				{
					GenericErrorData error2 = new GenericErrorData("strRobocloudError", "strItemShopPurchaseErrorTitle");
					ErrorWindow.ShowErrorWindow(error2);
					yield break;
				}
				url = request2.url;
				_loadingIconPresenter.NotifyLoading("XSollaShop", text);
			}
			_browserURLChangedObserver.AddAction(new ObserverAction<string>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_browserClosedObserver.AddAction((Action)OnBrowserClosed);
			_browser.Show(url);
		}

		private void ParseAddress(ref string url)
		{
			if (url.Contains("/desktop/status/"))
			{
				ExecutePurchaseEntityView purchaseRequest = GetPurchaseRequest();
				purchaseRequest.purchaseRequestComponent.purchaseConfirmed = true;
			}
			else if (url.Contains("robopay/started"))
			{
				_browser.Hide();
			}
		}

		private unsafe void OnBrowserClosed()
		{
			_browserURLChangedObserver.RemoveAction(new ObserverAction<string>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_browserClosedObserver.RemoveAction((Action)OnBrowserClosed);
			_loadingIconPresenter.NotifyLoadingDone("XSollaShop");
			ExecutePurchaseEntityView purchaseRequest = GetPurchaseRequest();
			bool purchaseConfirmed = purchaseRequest.purchaseRequestComponent.purchaseConfirmed;
			_purchaseRefresher.StartPollForPurchases(purchaseConfirmed);
			_guiInputController.SetShortCutMode(purchaseRequest.purchaseRequestComponent.previousShortcutMode);
			_entityFunctions.RemoveEntity<PurchaseRequestEntityDescriptor>(purchaseRequest.get_ID());
		}

		private ExecutePurchaseEntityView GetPurchaseRequest()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return entityViewsDB.QueryEntityViews<ExecutePurchaseEntityView>().get_Item(0);
		}
	}
}
