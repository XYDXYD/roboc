namespace ChatServiceLayer.Photon
{
	internal class ChatEventContainer : PhotonEventContainer<ChatEventCode>
	{
		public ChatEventContainer(ChatEventRegistry eventRegistry)
			: base((PhotonEventRegistry<ChatEventCode>)eventRegistry, (IPhotonClient)PhotonChatUtility.Instance.GetClient())
		{
		}
	}
}
