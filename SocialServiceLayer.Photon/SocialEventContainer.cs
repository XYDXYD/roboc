namespace SocialServiceLayer.Photon
{
	internal class SocialEventContainer : PhotonEventContainer<SocialEventCode>
	{
		public SocialEventContainer(SocialEventRegistry eventRegistry)
			: base((PhotonEventRegistry<SocialEventCode>)eventRegistry, PhotonSocialUtility.Instance.GetClientMustBeUsedOnlyForTheServiceContainer())
		{
		}

		public override void Disconnected()
		{
			if (CacheDTO.platoon.isInPlatoon)
			{
				CacheDTO.platoon = new Platoon();
				RaiseInternalEvent<PlatoonChangedEventListener, Platoon>(CacheDTO.platoon);
			}
			base.Disconnected();
		}
	}
}
