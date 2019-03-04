using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	public class YourClanEditingView : MonoBehaviour, IChainListener
	{
		private YourClanEditingController _controller;

		private SignalChain _signal;

		private BubbleSignal<IChainRoot> _bubble;

		public YourClanEditingView()
			: this()
		{
		}

		private void Start()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			_signal = new SignalChain(this.get_transform());
			_bubble = new BubbleSignal<IChainRoot>(this.get_transform().get_parent());
		}

		public void Listen(object message)
		{
			if (message is GenericComponentMessage)
			{
				GenericComponentMessage message2 = message as GenericComponentMessage;
				_controller.HandleGenericMessage(message2);
			}
		}

		internal void InjectController(YourClanEditingController controller)
		{
			_controller = controller;
		}

		public void DispatchGenericMessage(GenericComponentMessage message)
		{
			_signal.DeepBroadcast<GenericComponentMessage>(message);
		}

		public void BubbleUpSocialMessage(SocialMessage message)
		{
			_bubble.TargetedDispatch<SocialMessage>(message);
		}
	}
}
