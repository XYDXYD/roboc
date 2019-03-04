using ChatServiceLayer.Photon;

internal class ChatClientProvider
{
	public ChatClient GetClient()
	{
		return PhotonChatUtility.Instance.GetClient();
	}
}
