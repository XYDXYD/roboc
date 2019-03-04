using Services;

namespace SinglePlayerServiceLayer.Photon
{
	internal class SinglePlayerEventRegistry : PhotonEventRegistry<SinglePlayerEventCode>
	{
		public SinglePlayerEventRegistry(ISinglePlayerEventListenerFactory singlePlayerEventListenerFactory)
			: base((IEventListenerFactory)singlePlayerEventListenerFactory)
		{
			PhotonSinglePlayerUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer().SetEventRegistry(this);
		}
	}
}
