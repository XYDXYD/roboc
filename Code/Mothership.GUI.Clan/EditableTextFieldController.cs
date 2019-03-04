using Svelto.IoC;

namespace Mothership.GUI.Clan
{
	internal sealed class EditableTextFieldController
	{
		private EditableTextFieldView _view;

		private ShortCutMode _previousShortcutMode;

		private bool _previousInputControllerEnabled;

		[Inject]
		internal IGUIInputController guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal InputController inputController
		{
			private get;
			set;
		}

		public void SetView(EditableTextFieldView view)
		{
			_view = view;
		}

		public void OnTextFieldGetFocus()
		{
			_previousShortcutMode = guiInputController.GetShortCutMode();
			guiInputController.SetShortCutMode(ShortCutMode.NoKeyboardInputAllowed);
			_previousInputControllerEnabled = inputController.Enabled;
			inputController.Enabled = false;
		}

		public void OnTextFieldLoseFocus()
		{
			if (!UICamera.get_inputHasFocus())
			{
				guiInputController.SetShortCutMode(_previousShortcutMode);
				inputController.Enabled = _previousInputControllerEnabled;
			}
		}

		public void HandleMessage(object message)
		{
			if (message is SocialMessage)
			{
				SocialMessage socialMessage = message as SocialMessage;
				if (socialMessage.messageDispatched == SocialMessageType.ClanScreenClosed)
				{
					_view.UnfocusTextField();
				}
			}
		}
	}
}
