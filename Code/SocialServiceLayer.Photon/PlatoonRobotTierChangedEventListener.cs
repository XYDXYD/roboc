using ExitGames.Client.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class PlatoonRobotTierChangedEventListener : SocialEventListener<PlatoonRobotTierEventData>, IPlatoonRobotTierChangedEventListener, IServiceEventListener<PlatoonRobotTierEventData>, IServiceEventListenerBase
	{
		public override SocialEventCode SocialEventCode => SocialEventCode.PlatoonRobotTierChanged;

		protected override void ParseData(EventData eventData)
		{
			string b = (string)eventData.Parameters[16];
			string userName_ = (string)eventData.Parameters[18];
			int newRobotTier_ = (int)eventData.Parameters[20];
			if (CacheDTO.platoon != null && CacheDTO.platoon.isInPlatoon && CacheDTO.platoon.platoonId == b)
			{
				PlatoonRobotTierEventData data = new PlatoonRobotTierEventData(userName_, newRobotTier_);
				Invoke(data);
			}
		}
	}
}
