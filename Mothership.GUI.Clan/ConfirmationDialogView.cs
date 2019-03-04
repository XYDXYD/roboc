using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal class ConfirmationDialogView : MonoBehaviour, IChainListener, IGUIFactoryType
	{
		private ConfirmationDialogController _controller;

		public string viewName
		{
			get;
			set;
		}

		public Type guiElementFactoryType => typeof(ConfirmationDialogFactory);

		public ConfirmationDialogView()
			: this()
		{
		}

		public void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
				if (genericComponentMessage.Target == viewName)
				{
					_controller.HandleGenericMessage(genericComponentMessage);
				}
			}
		}

		public void InjectController(ConfirmationDialogController controller)
		{
			_controller = controller;
		}
	}
}
