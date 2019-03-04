using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Party
{
	public class PartyMemberClickForwarder : MonoBehaviour
	{
		public UIWidget button;

		public int index = -1;

		private BubbleSignal<PartyGUIView> _bubble;

		public PartyMemberClickForwarder()
			: this()
		{
		}

		private void Start()
		{
			_bubble = new BubbleSignal<PartyGUIView>(this.get_transform());
		}

		public void OnClick()
		{
			MouseButton mouseButton;
			switch (UICamera.currentTouchID)
			{
			case -1:
				mouseButton = MouseButton.LEFT;
				break;
			case -2:
				mouseButton = MouseButton.RIGHT;
				break;
			default:
				mouseButton = MouseButton.MIDDLE;
				break;
			}
			_bubble.TargetedDispatch<PartyMemberButtonClickMessage>(new PartyMemberButtonClickMessage(PartyIconState.MemberShowAvatar, button, index, mouseButton, string.Empty));
		}

		public void Dispatch(object message)
		{
			_bubble.TargetedDispatch<object>(message);
		}
	}
}
