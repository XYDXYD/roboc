using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	[Obsolete("Do not use this, coder must handle messaging between UIs...")]
	internal sealed class UIDisplayScreenListener : MonoBehaviour, IChainListener
	{
		[SerializeField]
		private GuiInputType inputType = GuiInputType.Toggle;

		[SerializeField]
		private GuiScreens screenToDisplay = GuiScreens.PauseMenu;

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		public UIDisplayScreenListener()
			: this()
		{
		}

		private void Start()
		{
		}

		public void Listen(object message)
		{
			if (!(message is ButtonType))
			{
				return;
			}
			ButtonType buttonType = (ButtonType)message;
			if (buttonType == ButtonType.Confirm)
			{
				switch (inputType)
				{
				case GuiInputType.Toggle:
					guiInputController.ToggleScreen(screenToDisplay);
					break;
				case GuiInputType.Show:
					guiInputController.ShowScreen(screenToDisplay);
					break;
				case GuiInputType.Hide:
					throw new Exception("Only a MVP can close a view");
				}
			}
		}
	}
}
