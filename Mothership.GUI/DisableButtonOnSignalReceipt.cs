using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI
{
	public class DisableButtonOnSignalReceipt : MonoBehaviour, IChainListener
	{
		private UIButton[] _buttons;

		public DisableButtonOnSignalReceipt()
			: this()
		{
		}

		public void Awake()
		{
			_buttons = this.GetComponents<UIButton>();
		}

		public void Listen(object message)
		{
			Type type = message.GetType();
			if (type == typeof(DisableButtonSignalMessage))
			{
				for (int i = 0; i < _buttons.Length; i++)
				{
					_buttons[i].set_isEnabled(false);
				}
			}
			else if (type == typeof(EnableButtonSignalMessage))
			{
				for (int j = 0; j < _buttons.Length; j++)
				{
					_buttons[j].set_isEnabled(true);
				}
			}
		}
	}
}
