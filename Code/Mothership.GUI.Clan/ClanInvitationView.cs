using Mothership.GUI.Social;
using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	internal class ClanInvitationView : MonoBehaviour, IChainListener, IChainRoot, IGUIFactoryType
	{
		[SerializeField]
		private GameObject inviteClanButtonTemplate;

		[SerializeField]
		private GameObject playerNameTextFieldTemplate;

		[SerializeField]
		private GameObject closeClanInvitationButtonTemplate;

		[SerializeField]
		private GameObject dismissSuccesfulPopupButtonTemplate;

		[SerializeField]
		private GameObject errorLabelTemplate;

		private ClanInvitationController _controller;

		private SignalChain _signal;

		private BubbleSignal<IChainRoot> _bubble;

		public GameObject inviteClanButton => inviteClanButtonTemplate;

		public GameObject playerNameTextField => playerNameTextFieldTemplate;

		public GameObject closeClanInvitationButton => closeClanInvitationButtonTemplate;

		public GameObject dismissSuccesfulPopupButton => dismissSuccesfulPopupButtonTemplate;

		public GameObject errorLabel => errorLabelTemplate;

		public Type guiElementFactoryType => typeof(ClanInvitationFactory);

		public ClanInvitationView()
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

		private void OnDisable()
		{
			this.get_gameObject().SetActive(false);
		}

		public void InjectController(ClanInvitationController controller)
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
