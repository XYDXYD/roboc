using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal sealed class ButtonView : MonoBehaviour, IChainListener, IGUIFactoryType
	{
		private ButtonController _controller;

		private UIButton[] _buttons;

		public Type guiElementFactoryType => typeof(ButtonFactory);

		public ButtonView()
			: this()
		{
		}

		private void Awake()
		{
			_buttons = this.GetComponentsInChildren<UIButton>();
		}

		public void InjectController(ButtonController controller)
		{
			_controller = controller;
		}

		public void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				_controller.HandleMessage(message as GenericComponentMessage);
			}
		}

		public void SetButtonActive(bool active)
		{
			for (int i = 0; i < _buttons.Length; i++)
			{
				_buttons[i].set_isEnabled(active);
			}
		}
	}
}
