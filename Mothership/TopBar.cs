using Fabric;
using Services.Analytics;
using Services.Requests.Interfaces;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal sealed class TopBar : MonoBehaviour, IChainListener, IInitialize
	{
		[SerializeField]
		private UITable buttonTable;

		[SerializeField]
		private Transform playerLevelInfoTransform;

		[SerializeField]
		private GameObject buttonBar;

		[SerializeField]
		private TabButton garageTabButton;

		[SerializeField]
		private TabButton playTabButton;

		[SerializeField]
		private TabButton inventoryTabButton;

		[SerializeField]
		private TabButton opsRoomTabButton;

		[SerializeField]
		private TabButton roboPassTabButton;

		[SerializeField]
		private TabButton itemShopTabButton;

		[SerializeField]
		private TabButton factoryTabButton;

		[SerializeField]
		private TabButton shopTabButton;

		[SerializeField]
		private GameObject purchasePremiumButton;

		[SerializeField]
		private GameObject purchaseCosmeticCreditsButton;

		private const string TOPBAR_APPEAR_ANIM = "TopBarAppear";

		private bool _isActive;

		private Animation _animation;

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
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
		internal LoadingIconPresenter loadingPresenter
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

		public Transform PlayerLevelInfoTransform => playerLevelInfoTransform;

		public TopBar()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			guiInputController.OnScreenStateChange += OnScreenStateChanged;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadPlatformConfigurationValues);
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				switch ((ButtonType)message)
				{
				case ButtonType.TopBarGarage:
					ResetTabButtons();
					garageTabButton.HighlightButton();
					guiInputController.ShowScreen(GuiScreens.Garage);
					break;
				case ButtonType.TopBarPlay:
					ResetTabButtons();
					playTabButton.HighlightButton();
					commandFactory.Build<SwitchToMainMenuCommand>().Execute();
					break;
				case ButtonType.TopBarInventory:
					ResetTabButtons();
					inventoryTabButton.HighlightButton();
					guiInputController.ShowScreen(GuiScreens.InventoryScreen);
					break;
				case ButtonType.TopBarOpsRoom:
					ResetTabButtons();
					opsRoomTabButton.HighlightButton();
					guiInputController.ShowScreen(GuiScreens.OpsRoom);
					break;
				case ButtonType.TopBarRoboPass:
					ResetTabButtons();
					roboPassTabButton.HighlightButton();
					guiInputController.ShowScreen(GuiScreens.RoboPassScreen);
					break;
				case ButtonType.TopBarItemShop:
					ResetTabButtons();
					itemShopTabButton.HighlightButton();
					guiInputController.ShowScreen(GuiScreens.ItemShopScreen);
					break;
				case ButtonType.TopBarFactory:
					ResetTabButtons();
					factoryTabButton.HighlightButton();
					guiInputController.ShowScreen(GuiScreens.RobotShop);
					break;
				case ButtonType.TopBarShop:
					throw new Exception("TopBarShop is removed now, use RealMoneyStore instead");
				case ButtonType.PurchasePremium:
					PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "StoreEntered", "TopBarPremium", startsNewChain: true);
					guiInputController.ShowScreen(GuiScreens.RealMoneyStoreScreen);
					break;
				case ButtonType.PurchaseCosmeticCredits:
					PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "StoreEntered", "TopBarCC", startsNewChain: true);
					guiInputController.ShowScreen(GuiScreens.RealMoneyStoreScreen);
					break;
				case ButtonType.TopBarRealMoneyStoreScreen:
					ResetTabButtons();
					shopTabButton.HighlightButton();
					PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "StoreEntered", "TopBarStore", startsNewChain: true);
					guiInputController.ShowScreen(GuiScreens.RealMoneyStoreScreen);
					break;
				}
			}
		}

		public void Build()
		{
			_animation = this.GetComponent<Animation>();
			Hide();
			this.get_gameObject().SetActive(false);
			garageTabButton.InitColours();
			playTabButton.InitColours();
			inventoryTabButton.InitColours();
			opsRoomTabButton.InitColours();
			roboPassTabButton.InitColours();
			itemShopTabButton.InitColours();
			factoryTabButton.InitColours();
			shopTabButton.InitColours();
			SetUpPurchaseButtons();
		}

		public void Hide()
		{
			_isActive = false;
		}

		public void Show()
		{
			_isActive = true;
		}

		public bool IsActive()
		{
			return _isActive;
		}

		public void SetDisplayStyle(TopBarStyle topBarStyle)
		{
			switch (topBarStyle)
			{
			case TopBarStyle.Default:
				SetDefault();
				break;
			case TopBarStyle.FullScreenInterface:
				SetFullScreenInterface();
				break;
			case TopBarStyle.TimerHidden:
				SetTimerHidden();
				break;
			case TopBarStyle.OffScreen:
				SetOffScreen();
				break;
			}
		}

		private void SetUpPurchaseButtons()
		{
			purchasePremiumButton.SetActive(false);
			purchaseCosmeticCreditsButton.SetActive(false);
		}

		private IEnumerator LoadPlatformConfigurationValues()
		{
			loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
			ILoadPlatformConfigurationRequest loadPlatformConfigurationReq = serviceFactory.Create<ILoadPlatformConfigurationRequest>();
			TaskService<PlatformConfigurationSettings> loadPlatformConfigurationTS = loadPlatformConfigurationReq.AsTask();
			HandleTaskServiceWithError handleTSWithError = new HandleTaskServiceWithError(loadPlatformConfigurationTS, delegate
			{
				loadingPresenter.NotifyLoading("LoadingPlatformConfiguration");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
			});
			yield return handleTSWithError.GetEnumerator();
			loadingPresenter.NotifyLoadingDone("LoadingPlatformConfiguration");
			if (loadPlatformConfigurationTS.succeeded)
			{
				PlatformConfigurationSettings result = loadPlatformConfigurationTS.result;
				shopTabButton.get_gameObject().SetActive(result.MainShopButtonAvailable);
				GameObject gameObject = roboPassTabButton.get_gameObject();
				gameObject.SetActive(result.RoboPassButtonAvailable);
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(loadPlatformConfigurationTS.behaviour);
			}
		}

		private void ResetTabButtons()
		{
			garageTabButton.ResetColours();
			playTabButton.ResetColours();
			inventoryTabButton.ResetColours();
			opsRoomTabButton.ResetColours();
			roboPassTabButton.ResetColours();
			itemShopTabButton.ResetColours();
			factoryTabButton.ResetColours();
			shopTabButton.ResetColours();
		}

		private void OnScreenStateChanged()
		{
			switch (guiInputController.GetActiveScreen())
			{
			case GuiScreens.Garage:
				ResetTabButtons();
				garageTabButton.HighlightButton();
				break;
			case GuiScreens.PlayScreen:
				ResetTabButtons();
				playTabButton.HighlightButton();
				break;
			case GuiScreens.InventoryScreen:
				ResetTabButtons();
				inventoryTabButton.HighlightButton();
				break;
			case GuiScreens.OpsRoom:
				ResetTabButtons();
				opsRoomTabButton.HighlightButton();
				break;
			case GuiScreens.RoboPassScreen:
				ResetTabButtons();
				roboPassTabButton.HighlightButton();
				break;
			case GuiScreens.ItemShopScreen:
				ResetTabButtons();
				itemShopTabButton.HighlightButton();
				break;
			case GuiScreens.RobotShop:
				ResetTabButtons();
				factoryTabButton.HighlightButton();
				break;
			case GuiScreens.RealMoneyStoreScreen:
				ResetTabButtons();
				shopTabButton.HighlightButton();
				break;
			}
			buttonTable.Reposition();
		}

		private void SetDefault()
		{
			this.get_gameObject().SetActive(true);
			if (!buttonBar.get_activeSelf())
			{
				EventManager.get_Instance().PostEvent("GUI_TopBar_Spawn", 0);
				_animation.Play("TopBarAppear");
			}
			buttonBar.SetActive(true);
			PlayerPrefs.SetInt("AccessedMenuTAB", 1);
		}

		private void SetFullScreenInterface()
		{
			this.get_gameObject().SetActive(true);
			if (!buttonBar.get_activeSelf())
			{
				EventManager.get_Instance().PostEvent("GUI_TopBar_Spawn", 0);
				_animation.Play("TopBarAppear");
			}
			buttonBar.SetActive(true);
			Show();
		}

		private void SetTimerHidden()
		{
			this.get_gameObject().SetActive(true);
			buttonBar.SetActive(false);
		}

		private void SetOffScreen()
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
