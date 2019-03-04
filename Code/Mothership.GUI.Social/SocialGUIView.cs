using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal class SocialGUIView : MonoBehaviour, IChainListener, IChainRoot, ISocialRoot
	{
		private SocialGUIController _controller;

		private SignalChain _signal;

		public SocialGUIView()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			_signal = new SignalChain(this.get_transform());
		}

		public void InjectController(SocialGUIController controller)
		{
			_controller = controller;
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		public void DeepBroadcast<T>(T message)
		{
			_signal.DeepBroadcast<T>(message);
		}

		private void Update()
		{
			_controller.Tick();
		}
	}
}
