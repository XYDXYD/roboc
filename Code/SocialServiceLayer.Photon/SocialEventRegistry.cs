using Services;

namespace SocialServiceLayer.Photon
{
	internal class SocialEventRegistry : PhotonEventRegistry<SocialEventCode>
	{
		public SocialEventRegistry(ISocialEventListenerFactory socialEventListenerFactory)
			: base((IEventListenerFactory)socialEventListenerFactory)
		{
			PhotonSocialUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer().SetEventRegistry(this);
		}
	}
}
