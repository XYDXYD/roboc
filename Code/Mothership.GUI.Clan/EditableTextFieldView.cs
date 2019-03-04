using Mothership.GUI.Social;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal class EditableTextFieldView : MonoBehaviour, IGUIFactoryType, IChainListener
	{
		private UIInputWithFocusEvents _textField;

		private EditableTextFieldController _controller;

		public Type guiElementFactoryType => typeof(EditableTextFieldControllerFactory);

		public EditableTextFieldView()
			: this()
		{
		}

		private void Awake()
		{
			_textField = this.GetComponent<UIInputWithFocusEvents>();
			_textField.OnInputGetFocus += HandleOnInputGetFocus;
			_textField.OnInputLoseFocus += HandleOnInputLoseFocus;
		}

		public void InjectController(EditableTextFieldController controller)
		{
			_controller = controller;
		}

		private void HandleOnInputLoseFocus()
		{
			_controller.OnTextFieldLoseFocus();
		}

		private void HandleOnInputGetFocus()
		{
			_controller.OnTextFieldGetFocus();
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		internal void UnfocusTextField()
		{
			if (_textField.get_isSelected())
			{
				_textField.set_isSelected(false);
			}
		}
	}
}
