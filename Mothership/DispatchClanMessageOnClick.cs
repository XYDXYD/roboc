using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal sealed class DispatchClanMessageOnClick : MonoBehaviour
	{
		public string extraData;

		public SocialMessageType messageType;

		private SocialMessage _message;

		private BubbleSignal<IChainRoot> _bubble;

		public DispatchClanMessageOnClick()
			: this()
		{
		}

		private void Start()
		{
			_message = new SocialMessage(messageType, extraData);
		}

		private void OnClick()
		{
			_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
			_bubble.Dispatch<SocialMessage>(_message);
		}
	}
}
