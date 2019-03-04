using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonDisbandedEventListener : SocialEventListener, IPlatoonDisbandedEventListener, IServiceEventListener, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.PlatoonDisbanded;

		protected override void ParseData(EventData eventData)
		{
			string b = (string)eventData.Parameters[16];
			if (CacheDTO.platoon.isInPlatoon && CacheDTO.platoon.platoonId == b)
			{
				CacheDTO.platoon = new Platoon();
				Invoke();
				PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
			}
		}
	}
}
