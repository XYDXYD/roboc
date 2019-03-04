using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Login
{
	internal class SplashLoginErrorDialogView : MonoBehaviour, ISplashLoginDialogView, IChainListener
	{
		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UILabel bodyText;

		[SerializeField]
		private GameObject graphicHolder;

		[SerializeField]
		private UILabel[] oneButtonLayout_buttonLabels;

		[SerializeField]
		private UILabel[] twoButtonLayout_buttonLabels;

		[SerializeField]
		private UILabel[] threeButtonLayout_buttonLabels;

		[SerializeField]
		private DispatchSplashLoginMessageOnClick[] oneButtonLayout_buttonMessages;

		[SerializeField]
		private DispatchSplashLoginMessageOnClick[] twoButtonLayout_buttonMessages;

		[SerializeField]
		private DispatchSplashLoginMessageOnClick[] threeButtonLayout_buttonMessages;

		[SerializeField]
		private GameObject oneButtonLayout;

		[SerializeField]
		private GameObject twoButtonLayout;

		[SerializeField]
		private GameObject threeButtonLayout;

		private SplashLoginErrorDialogController _controller;

		public SplashLoginErrorDialogView()
			: this()
		{
		}

		void ISplashLoginDialogView.InjectController(ISplashLoginDialogController controller)
		{
			_controller = (controller as SplashLoginErrorDialogController);
		}

		public void DestroySelf()
		{
			this.get_gameObject().get_transform().set_parent(null);
			Object.Destroy(this.get_gameObject());
		}

		public void SetTitleText(string title_)
		{
			title.set_text(title_);
		}

		public void SetBodyText(string bodyText_)
		{
			bodyText.set_text(bodyText_);
		}

		public SplashLoginGUIMessageType GetMessageTypeForButtonType(SplashLoginErrorDialogConfiguration.ButtonType buttonType)
		{
			switch (buttonType)
			{
			case SplashLoginErrorDialogConfiguration.ButtonType.OK:
				return SplashLoginGUIMessageType.ErrorButtonOKPressed;
			case SplashLoginErrorDialogConfiguration.ButtonType.Cancel:
				return SplashLoginGUIMessageType.ErrorButtonCancelPressed;
			case SplashLoginErrorDialogConfiguration.ButtonType.Quit:
				return SplashLoginGUIMessageType.ErrorButtonQuitPressed;
			case SplashLoginErrorDialogConfiguration.ButtonType.Retry:
				return SplashLoginGUIMessageType.ErrorButtonRetryPressed;
			default:
				return SplashLoginGUIMessageType.ErrorButtonOKPressed;
			}
		}

		private string GetStringForButtonType(SplashLoginErrorDialogConfiguration.ButtonType buttonType)
		{
			switch (buttonType)
			{
			case SplashLoginErrorDialogConfiguration.ButtonType.OK:
				return StringTableBase<StringTable>.Instance.GetString("strOK");
			case SplashLoginErrorDialogConfiguration.ButtonType.Cancel:
				return StringTableBase<StringTable>.Instance.GetString("strCancel");
			case SplashLoginErrorDialogConfiguration.ButtonType.Quit:
				return StringTableBase<StringTable>.Instance.GetString("strQuit");
			case SplashLoginErrorDialogConfiguration.ButtonType.Retry:
				return StringTableBase<StringTable>.Instance.GetString("strRetry");
			default:
				return string.Empty;
			}
		}

		public void SetOneButtonLayout(SplashLoginErrorDialogConfiguration.ButtonType buttonType1)
		{
			oneButtonLayout.SetActive(true);
			twoButtonLayout.SetActive(false);
			threeButtonLayout.SetActive(false);
			string stringForButtonType = GetStringForButtonType(buttonType1);
			oneButtonLayout_buttonLabels[0].set_text(stringForButtonType);
			oneButtonLayout_buttonMessages[0].MessageType = GetMessageTypeForButtonType(buttonType1);
		}

		public void SetTwoButtonLayout(SplashLoginErrorDialogConfiguration.ButtonType buttonType1, SplashLoginErrorDialogConfiguration.ButtonType buttonType2)
		{
			oneButtonLayout.SetActive(false);
			twoButtonLayout.SetActive(true);
			threeButtonLayout.SetActive(false);
			string stringForButtonType = GetStringForButtonType(buttonType1);
			oneButtonLayout_buttonLabels[0].set_text(stringForButtonType);
			oneButtonLayout_buttonMessages[0].MessageType = GetMessageTypeForButtonType(buttonType1);
			stringForButtonType = GetStringForButtonType(buttonType2);
			oneButtonLayout_buttonLabels[1].set_text(stringForButtonType);
			oneButtonLayout_buttonMessages[1].MessageType = GetMessageTypeForButtonType(buttonType2);
		}

		public void Listen(object message)
		{
			if (message is SplashLoginGUIMessage)
			{
				SplashLoginGUIMessageType message2 = (message as SplashLoginGUIMessage).Message;
				_controller.HandleButtonClick(message2);
			}
		}
	}
}
