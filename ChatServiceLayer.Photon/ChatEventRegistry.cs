using Services;

namespace ChatServiceLayer.Photon
{
	internal class ChatEventRegistry : PhotonEventRegistry<ChatEventCode>
	{
		public ChatEventRegistry(IChatEventListenerFactory chatEventListenerFactory)
			: base((IEventListenerFactory)chatEventListenerFactory)
		{
			PhotonChatUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer().SetEventRegistry(this);
		}
	}
}
