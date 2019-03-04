using Mothership.GUI.Social;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class ClanScreen : MonoBehaviour, IInitialize, IChainListener, IChainRoot, IAnchorUISource, IGUIFactoryType, IClanRoot
	{
		private bool _isActive;

		public GameObject ClanMainContainer;

		public GameObject ClanMenuTabs;

		public GameObject ClanMenuSingleTabOnTop;

		public GameObject ClanMenuSingleTabOnTopCloseButton;

		public GameObject ClanMenuSingleTabOnTopBackButton;

		public GameObject ClanMainContainerWithoutMenu;

		public UILabel[] MenuTabs1;

		public UILabel[] MenuTabs2;

		public GameObject[] tabButtonsActiveState;

		public GameObject[] tabButtonsInactiveState;

		public int screenSizeSwitchHeight = 720;

		public int smallScreenTop = -80;

		public int largeScreenTop = -185;

		private bool _buttonVisibility;

		private BubbleSignal<ISocialRoot> _bubble;

		private UIWidget _mainContainerWidget;

		private int _lastScreenWidth;

		private int _lastScreenHeight;

		[Inject]
		internal ClanController clanController
		{
			private get;
			set;
		}

		public Type guiElementFactoryType => typeof(ClanGeneralGUIFactory);

		public ClanScreen()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			clanController.SetMainView(this);
		}

		private void Awake()
		{
			_bubble = new BubbleSignal<ISocialRoot>(this.get_transform());
			_mainContainerWidget = ClanMainContainer.GetComponent<UIWidget>();
			SetSingleTabHeaderVisible(tabHeaderVisible: false);
			SetTabsVisibile(status: true);
			HideMainPanel();
		}

		public void ShowMainPanel()
		{
			_isActive = true;
			ClanMainContainer.SetActive(true);
		}

		public void HideMainPanel()
		{
			if (!_buttonVisibility)
			{
				_isActive = false;
			}
			ClanMainContainer.SetActive(false);
		}

		public void SetTabsVisibile(bool status)
		{
			ClanMenuTabs.SetActive(status);
		}

		public void SetSingleTabActive(int tabIndex)
		{
			for (int i = 0; i < tabButtonsActiveState.Length; i++)
			{
				if (i == tabIndex)
				{
					tabButtonsActiveState[i].SetActive(true);
					tabButtonsInactiveState[i].SetActive(false);
				}
				else
				{
					tabButtonsActiveState[i].SetActive(false);
					tabButtonsInactiveState[i].SetActive(true);
				}
			}
		}

		public void SetSingleTabHeaderVisible(bool tabHeaderVisible)
		{
			ClanMenuSingleTabOnTop.SetActive(tabHeaderVisible);
		}

		public bool IsActive()
		{
			return _isActive;
		}

		public void SetTabLabel(int tabIndex, string label)
		{
			if (tabIndex == 0)
			{
				for (int i = 0; i < MenuTabs1.Length; i++)
				{
					MenuTabs1[i].set_text(label);
				}
			}
			if (tabIndex == 1)
			{
				for (int j = 0; j < MenuTabs2.Length; j++)
				{
					MenuTabs2[j].set_text(label);
				}
			}
		}

		public void SendClanMessage(SocialMessage message)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			new SignalChain(this.get_transform()).DeepBroadcast(typeof(SocialMessage), (object)message);
		}

		public void BubbleUpSocialMessage(SocialMessage message)
		{
			_bubble.TargetedDispatch<SocialMessage>(message);
		}

		public void Listen(object message)
		{
			if (message.GetType() == typeof(SocialMessage))
			{
				clanController.HandleClanMessage(message as SocialMessage);
			}
		}

		public UIRect GetAnchorSource()
		{
			return ClanMainContainerWithoutMenu.GetComponent<UIWidget>();
		}

		private void Update()
		{
			if (Screen.get_width() != _lastScreenWidth || Screen.get_height() != _lastScreenHeight)
			{
				clanController.OnScreenSizeChange(Screen.get_height(), _lastScreenHeight);
				_lastScreenWidth = Screen.get_width();
				_lastScreenHeight = Screen.get_height();
			}
		}

		public void Maximize()
		{
			_mainContainerWidget.topAnchor.relative = 1f;
			_mainContainerWidget.topAnchor.absolute = smallScreenTop;
			_mainContainerWidget.UpdateAnchors();
		}

		public void Minimize()
		{
			_mainContainerWidget.topAnchor.relative = 1f;
			_mainContainerWidget.topAnchor.absolute = largeScreenTop;
			_mainContainerWidget.UpdateAnchors();
		}
	}
}
