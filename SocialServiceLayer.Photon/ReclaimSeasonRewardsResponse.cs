namespace SocialServiceLayer.Photon
{
	internal class ReclaimSeasonRewardsResponse
	{
		public readonly int robitsRewarded;

		public readonly int seasonNumberOfReward;

		public readonly long newRobitsTotal;

		public ReclaimSeasonRewardsResponse(long newRobitsTotal_, int robitsRewarded_, int seasonNumberOfReward_)
		{
			robitsRewarded = robitsRewarded_;
			newRobitsTotal = newRobitsTotal_;
			seasonNumberOfReward = seasonNumberOfReward_;
		}
	}
}
