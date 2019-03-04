using Mothership.GUI.Social;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class FriendScreen : MonoBehaviour, IInitialize, IChainListener, IChainRoot, IAnchorUISource, IGUIFactoryType
	{
		public GameObject FriendMainContainer;

		public int screenSizeSwitchHeight = 720;

		public int smallScreenTop = -80;

		public int largeScreenTop = -185;

		private bool _isActive;

		private bool _buttonVisibility;

		private BubbleSignal<ISocialRoot> _socialBubble;

		private UIWidget _mainContainerWidget;

		private int _lastScreenWidth;

		private int _lastScreenHeight;

		[Inject]
		internal FriendController friendController
		{
			private get;
			set;
		}

		public Type guiElementFactoryType => typeof(FriendGeneralGUIFactory);

		public FriendScreen()
			: this()
		{
		}

		public void ShowMainPanel()
		{
			_isActive = true;
			FriendMainContainer.SetActive(true);
		}

		public void HideMainPanel()
		{
			if (!_buttonVisibility)
			{
				_isActive = false;
			}
			FriendMainContainer.SetActive(false);
		}

		public bool IsActive()
		{
			return _isActive;
		}

		public void SendFriendMessage(SocialMessage message)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			new SignalChain(this.get_transform()).DeepBroadcast(typeof(SocialMessage), (object)message);
		}

		public void BubbleSocialMessage(SocialMessage message)
		{
			_socialBubble.TargetedDispatch<SocialMessage>(message);
		}

		public void Listen(object message)
		{
			if (message.GetType() == typeof(SocialMessage))
			{
				friendController.HandleSocialMessage(message as SocialMessage);
			}
		}

		public UIRect GetAnchorSource()
		{
			return FriendMainContainer.GetComponent<UIWidget>();
		}

		private void Awake()
		{
			_mainContainerWidget = FriendMainContainer.GetComponent<UIWidget>();
			_socialBubble = new BubbleSignal<ISocialRoot>(this.get_transform());
		}

		void IInitialize.OnDependenciesInjected()
		{
			friendController.SetMainView(this);
		}

		private void Update()
		{
			if (Screen.get_width() != _lastScreenWidth || Screen.get_height() != _lastScreenHeight)
			{
				friendController.OnScreenSizeChange(Screen.get_height(), _lastScreenHeight);
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
