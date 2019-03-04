using UnityEngine;

namespace Robocraft.GUI
{
	internal sealed class DispatchGenericMessageOnClick : MonoBehaviour
	{
		public string target;

		public string originator;

		public MessageType messageType;

		private GenericComponentMessage _message;

		private GenericMessagePropogator _messageBubble;

		public DispatchGenericMessageOnClick()
			: this()
		{
		}

		private void Start()
		{
			_message = new GenericComponentMessage(messageType, originator, target);
			_messageBubble = new GenericMessagePropogator(this.get_transform());
		}

		private void OnClick()
		{
			_messageBubble.SendMessageUpTree(_message);
		}
	}
}
