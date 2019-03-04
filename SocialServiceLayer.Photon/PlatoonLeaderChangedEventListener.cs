using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonLeaderChangedEventListener : SocialEventListener<string>, IPlatoonLeaderChangedEventListener, IServiceEventListener<string>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.PlatoonLeaderChanged;

		protected override void ParseData(EventData eventData)
		{
			string leader = (string)eventData.Parameters[17];
			string data = (string)eventData.Parameters[75];
			CacheDTO.platoon.leader = leader;
			Invoke(data);
			PhotonSocialUtility.Instance.RaiseInternalEvent<IPlatoonChangedEventListener, Platoon>((Platoon)CacheDTO.platoon.Clone());
		}
	}
}
