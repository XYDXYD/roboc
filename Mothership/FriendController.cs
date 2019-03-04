using Svelto.IoC;
using Svelto.ServiceLayer;
using System;

namespace Mothership
{
	internal sealed class FriendController : IFloatingWidget
	{
		public Action<IAnchorUISource> OnMainViewSet = delegate
		{
		};

		private FriendScreen _mainView;

		private ShortCutMode _previousShortcutMode;

		[Inject]
		public IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public void SetMainView(FriendScreen view)
		{
			_mainView = view;
		}

		public bool IsActive()
		{
			return _mainView.IsActive();
		}

		public GUIShowResult Show()
		{
			OnMainViewSet(_mainView);
			_mainView.HideMainPanel();
			return GUIShowResult.Showed;
		}

		public void HandleSocialMessage(SocialMessage message)
		{
			switch (message.messageDispatched)
			{
			case SocialMessageType.MaximizeFriendScreen:
				Open();
				break;
			case SocialMessageType.MaximizeClanScreen:
				Close();
				break;
			case SocialMessageType.ClickedOutsideSocial:
				Close();
				break;
			case SocialMessageType.CloseSocialScreens:
				if (message.extraDetails == "ChatRoot")
				{
					Close();
				}
				break;
			}
		}

		private void Open()
		{
			if (!_mainView.IsActive())
			{
				guiInputController.AddFloatingWidget(this);
				_mainView.ShowMainPanel();
			}
			else
			{
				Close();
			}
		}

		private void Close()
		{
			if (_mainView.IsActive())
			{
				guiInputController.RemoveFloatingWidget(this);
				_mainView.HideMainPanel();
				_mainView.BubbleSocialMessage(new SocialMessage(SocialMessageType.FriendScreenClosed, string.Empty));
			}
		}

		public void DispatchAnyFriendMessage(SocialMessage message)
		{
			if (message != null)
			{
				_mainView.SendFriendMessage(message);
			}
		}

		public void OnScreenSizeChange(int newHeight, int oldHeight)
		{
			if (newHeight > _mainView.screenSizeSwitchHeight && oldHeight <= _mainView.screenSizeSwitchHeight)
			{
				_mainView.Minimize();
			}
			else if (newHeight <= _mainView.screenSizeSwitchHeight && oldHeight > _mainView.screenSizeSwitchHeight)
			{
				_mainView.Maximize();
			}
		}

		public void HandleQuitPressed()
		{
			Close();
		}
	}
}
