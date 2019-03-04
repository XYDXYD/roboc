using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	public class DispatchOnEditableTextFieldChange : MonoBehaviour
	{
		public SocialMessageType messageType;

		private SocialMessage _message;

		private BubbleSignal<IChainRoot> _bubble;

		public DispatchOnEditableTextFieldChange()
			: this()
		{
		}

		private void Start()
		{
			_message = new SocialMessage(messageType, string.Empty);
			_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
		}

		public void OnChanged()
		{
			if (UIInput.current != null)
			{
				_message.extraDetails = UIInput.current.get_value();
				_bubble.Dispatch<SocialMessage>(_message);
			}
		}
	}
}
