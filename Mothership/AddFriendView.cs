using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership
{
	internal class AddFriendView : MonoBehaviour, IGUIFactoryType, IChainListener, IChainRoot
	{
		[SerializeField]
		private GameObject inviteFriendButtonTemplate;

		[SerializeField]
		private GameObject playerNameTextFieldTemplate;

		[SerializeField]
		private GameObject closeFriendInvitationButtonTemplate;

		[SerializeField]
		private GameObject dismissButtonTemplate;

		[SerializeField]
		private GameObject errorLabelTemplate;

		private AddFriendController _controller;

		private SignalChain _signal;

		private BubbleSignal<IChainRoot> _bubble;

		public GameObject inviteFriendButton => inviteFriendButtonTemplate;

		public GameObject playerNameTextField => playerNameTextFieldTemplate;

		public GameObject closeFriendInvitationButton => closeFriendInvitationButtonTemplate;

		public GameObject dismissButton => dismissButtonTemplate;

		public GameObject errorLabel => errorLabelTemplate;

		public Type guiElementFactoryType => typeof(AddFriendFactory);

		public AddFriendView()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			_signal = new SignalChain(this.get_transform());
			_bubble = new BubbleSignal<IChainRoot>(this.get_transform().get_parent());
		}

		public void InjectController(AddFriendController controller)
		{
			_controller = controller;
		}

		public void Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		public void BroadcastDownGenericMessage(GenericComponentMessage message)
		{
			_signal.DeepBroadcast(typeof(GenericComponentMessage), (object)message);
		}

		public void BubbleUpGenericMessage(GenericComponentMessage message)
		{
			_bubble.TargetedDispatch<GenericComponentMessage>(message);
		}
	}
}
