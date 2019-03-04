using Services;

namespace LobbyServiceLayer.Photon
{
	internal class LobbyEventRegistry : PhotonEventRegistry<LobbyEventCode>
	{
		public LobbyEventRegistry(ILobbyEventListenerFactory lobbyEventListenerFactory)
			: base((IEventListenerFactory)lobbyEventListenerFactory)
		{
			PhotonLobbyUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer().SetEventRegistry(this);
		}
	}
}
