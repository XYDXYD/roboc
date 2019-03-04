using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	public class DispatchOnPopupListChange : MonoBehaviour
	{
		public SocialMessageType messageType;

		private SocialMessage _message;

		private BubbleSignal<IChainRoot> _bubble;

		public DispatchOnPopupListChange()
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
			if (UIPopupList.current != null)
			{
				_message.extraData = UIPopupList.current.get_data();
				_bubble.Dispatch<SocialMessage>(_message);
			}
		}
	}
}
