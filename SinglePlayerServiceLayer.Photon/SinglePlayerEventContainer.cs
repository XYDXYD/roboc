namespace SinglePlayerServiceLayer.Photon
{
	internal class SinglePlayerEventContainer : PhotonEventContainer<SinglePlayerEventCode>
	{
		public SinglePlayerEventContainer(SinglePlayerEventRegistry eventRegistry)
			: base((PhotonEventRegistry<SinglePlayerEventCode>)eventRegistry, PhotonSinglePlayerUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer())
		{
		}
	}
}
