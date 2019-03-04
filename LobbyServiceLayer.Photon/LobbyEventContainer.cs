namespace LobbyServiceLayer.Photon
{
	internal class LobbyEventContainer : PhotonEventContainer<LobbyEventCode>
	{
		public LobbyEventContainer(LobbyEventRegistry eventRegistry)
			: base((PhotonEventRegistry<LobbyEventCode>)eventRegistry, PhotonLobbyUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer())
		{
		}
	}
}
