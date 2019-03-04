using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Party
{
	public class PartyGUIView : MonoBehaviour, IChainListener, IInitialize, IChainRoot, IPartyGUIViewRoot
	{
		public UIWidget panelBounds;

		public GameObject mainContent;

		public GameObject partyInvitation;

		public PartyGUIStyle[] styles;

		public int panelHeight;

		public UIWidget inviteDropDownArea;

		private SignalChain _signal;

		[Inject]
		internal PartyGUIController partyGUIController
		{
			private get;
			set;
		}

		public PartyGUIView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			partyGUIController.RegisterView(this);
		}

		void IChainListener.Listen(object message)
		{
			partyGUIController.ReceiveMessage(message);
		}

		public void BuildSignal()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			_signal = new SignalChain(this.get_transform());
		}

		public void Broadcast(object message)
		{
			_signal.Broadcast<object>(message);
		}

		public void DeepBroadcast<T>(T message)
		{
			if (_signal != null)
			{
				_signal.DeepBroadcast<T>(message);
			}
		}
	}
}
