using Robocraft.GUI;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.GUI.Clan
{
	public class ShowHideIfGenericMessageReceived : MonoBehaviour, IChainListener
	{
		[SerializeField]
		private string objectName;

		[SerializeField]
		private MessageType showMessageType;

		[SerializeField]
		private MessageType hideMessageType;

		public ShowHideIfGenericMessageReceived()
			: this()
		{
		}

		public void Listen(object message)
		{
			if (!(message is GenericComponentMessage))
			{
				return;
			}
			GenericComponentMessage genericComponentMessage = message as GenericComponentMessage;
			if (genericComponentMessage.Target == objectName)
			{
				if (genericComponentMessage.Message == showMessageType)
				{
					this.get_gameObject().SetActive(true);
				}
				else if (genericComponentMessage.Message == hideMessageType)
				{
					this.get_gameObject().SetActive(false);
				}
			}
		}
	}
}
