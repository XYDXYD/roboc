namespace SocialServiceLayer.Photon
{
	internal class PlatoonRobotTierEventData
	{
		public readonly string UserName;

		public readonly int NewRobotTier;

		public PlatoonRobotTierEventData(string userName_, int newRobotTier_)
		{
			UserName = userName_;
			NewRobotTier = newRobotTier_;
		}
	}
}
