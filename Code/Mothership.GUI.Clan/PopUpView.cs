using Mothership.GUI.Social;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal class PopUpView : MonoBehaviour, IChainListener, IGUIFactoryType
	{
		[SerializeField]
		private string name;

		private PopUpController _controller;

		public string popUpName => name;

		public Type guiElementFactoryType => typeof(PopUpFactory);

		public PopUpView()
			: this()
		{
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		public void InjectController(PopUpController controller)
		{
			_controller = controller;
		}
	}
}
