using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Login
{
	internal sealed class DispatchSplashLoginMessageOnClick : MonoBehaviour
	{
		public SplashLoginGUIMessageType MessageType;

		private BubbleSignal<IChainRoot> _bubble;

		private SplashLoginGUIMessage _message;

		public DispatchSplashLoginMessageOnClick()
			: this()
		{
		}

		public void OnClick()
		{
			_bubble = new BubbleSignal<IChainRoot>(this.get_transform());
			_bubble.Dispatch<SplashLoginGUIMessage>(_message);
		}

		private void Start()
		{
			_message = new SplashLoginGUIMessage(MessageType);
		}
	}
}
