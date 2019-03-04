using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Login
{
	internal class EnterUsernameDialogView : MonoBehaviour, ISplashLoginDialogView, IChainListener
	{
		[SerializeField]
		private UIInput textEntryField;

		[SerializeField]
		private UILabel textEntryNotAllowedMessage;

		[SerializeField]
		private UILabel textEntryAllowedMessage;

		[SerializeField]
		private UILabel characterLimitInformation;

		private const string CharacterLimitText = "strTencentCharacterLimit";

		private const string UsernameProfanityText = "strTencentUsernameProfanity";

		private const string UsernameTooLongText = "strTencentUsernameTooLong";

		private const string UsernameInUseText = "strTencentUsernameInUse";

		private const string InvalidUsernameText = "strTencentInvalidUsername";

		private const string UsernameTooShortText = "strTencentUsernameTooShort";

		private SignalChain _signal;

		private EnterUsernameDialogController _controller;

		public string IdentifierEntry => textEntryField.get_value().ToLower();

		public EnterUsernameDialogView()
			: this()
		{
		}

		void ISplashLoginDialogView.InjectController(ISplashLoginDialogController controller)
		{
			_controller = (controller as EnterUsernameDialogController);
		}

		public unsafe void Start()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Expected O, but got Unknown
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			characterLimitInformation.set_text(StringTableBase<StringTable>.Instance.GetString("strTencentCharacterLimit"));
			RebuildSignalChain();
			textEntryField.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			HideTextEntryMessages();
		}

		public void RebuildSignalChain()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			GameObject gameObject = this.get_gameObject();
			while (gameObject.get_transform().get_parent() != null)
			{
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
				if (gameObject.GetComponent<IChainRoot>() != null)
				{
					break;
				}
			}
			_signal = new SignalChain(gameObject.get_transform());
		}

		public void HideTextEntryMessages()
		{
			textEntryNotAllowedMessage.get_gameObject().SetActive(false);
			textEntryAllowedMessage.get_gameObject().SetActive(false);
		}

		public void HideTextEntryInvalidMessage()
		{
			textEntryNotAllowedMessage.get_gameObject().SetActive(false);
			textEntryAllowedMessage.get_gameObject().SetActive(true);
		}

		public void ShowTextEntryInvalidMessage(UsernameValidDuringEntryStatus status, string message)
		{
			textEntryNotAllowedMessage.get_gameObject().SetActive(true);
			textEntryAllowedMessage.get_gameObject().SetActive(false);
			textEntryNotAllowedMessage.set_text(message);
		}

		public void ShowTextEntryInvalidMessage(UsernameValidDuringEntryStatus status)
		{
			if (status == UsernameValidDuringEntryStatus.EntryIsValid)
			{
				textEntryNotAllowedMessage.get_gameObject().SetActive(false);
				textEntryAllowedMessage.get_gameObject().SetActive(true);
				return;
			}
			textEntryNotAllowedMessage.get_gameObject().SetActive(true);
			textEntryAllowedMessage.get_gameObject().SetActive(false);
			switch (status)
			{
			case UsernameValidDuringEntryStatus.EntryContainsProfanity:
				textEntryNotAllowedMessage.set_text(StringTableBase<StringTable>.Instance.GetString("strTencentUsernameProfanity"));
				break;
			case UsernameValidDuringEntryStatus.EntryTooLong:
				textEntryNotAllowedMessage.set_text(StringTableBase<StringTable>.Instance.GetString("strTencentUsernameTooLong"));
				break;
			case UsernameValidDuringEntryStatus.NameAlreadyUsed:
				textEntryNotAllowedMessage.set_text(StringTableBase<StringTable>.Instance.GetString("strTencentUsernameInUse"));
				break;
			case UsernameValidDuringEntryStatus.EntryIsInvalidSomeOtherReasons:
				textEntryNotAllowedMessage.set_text(StringTableBase<StringTable>.Instance.GetString("strTencentInvalidUsername"));
				break;
			case UsernameValidDuringEntryStatus.EntryTooShort:
				textEntryNotAllowedMessage.set_text(StringTableBase<StringTable>.Instance.GetString("strTencentUsernameTooShort"));
				break;
			}
		}

		private void HandleOnChangeEvent()
		{
			_signal.DeepBroadcast(typeof(SplashLoginGUIMessage), (object)new SplashLoginGUIMessage(SplashLoginGUIMessageType.EnterUsernameTextEntryFieldChanged));
		}

		public void DestroySelf()
		{
			Object.Destroy(this.get_gameObject());
		}

		public void Listen(object message)
		{
			if (message is SplashLoginGUIMessage)
			{
				SplashLoginGUIMessage splashLoginGUIMessage = message as SplashLoginGUIMessage;
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ChangedTextEntryIsValid)
				{
					_controller.SetTextEntryIsValid(UsernameValidDuringEntryStatus.EntryIsValid);
				}
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ChangedTextEntryContainsProfanity)
				{
					_controller.SetTextEntryIsValid(UsernameValidDuringEntryStatus.EntryContainsProfanity);
				}
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ChangedTextEntryIsTooLong)
				{
					_controller.SetTextEntryIsValid(UsernameValidDuringEntryStatus.EntryTooLong);
				}
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ChangedTextEntryIsTooShort)
				{
					_controller.SetTextEntryIsValid(UsernameValidDuringEntryStatus.EntryTooShort);
				}
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ChangedTextEntryNameInUse)
				{
					_controller.SetTextEntryIsValid(UsernameValidDuringEntryStatus.NameAlreadyUsed);
				}
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ChangedTextEntrySomeOtherError)
				{
					_controller.SetTextEntryIsValid(UsernameValidDuringEntryStatus.EntryIsInvalidSomeOtherReasons);
				}
			}
		}
	}
}
