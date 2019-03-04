using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Login
{
	internal class SplashLoginSteamRobocraftChoiceView : MonoBehaviour, ISplashLoginDialogView, IChainListener
	{
		private SplashLoginSteamRobocraftChoiceController _controller;

		public SplashLoginSteamRobocraftChoiceView()
			: this()
		{
		}

		void ISplashLoginDialogView.InjectController(ISplashLoginDialogController controller)
		{
			_controller = (controller as SplashLoginSteamRobocraftChoiceController);
		}

		public void DestroySelf()
		{
			this.get_gameObject().get_transform().set_parent(null);
			Object.Destroy(this.get_gameObject());
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}
	}
}
