using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class KickedFromPlatoonEventListener : SocialEventListener, IKickedFromPlatoonEventListener, IServiceEventListener, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.KickedFromPlatoon;

		protected override void ParseData(EventData eventData)
		{
			CacheDTO.platoon = new Platoon();
			Invoke();
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
		}
	}
}
