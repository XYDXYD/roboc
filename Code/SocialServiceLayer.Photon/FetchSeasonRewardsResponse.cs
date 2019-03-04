namespace SocialServiceLayer.Photon
{
	internal class FetchSeasonRewardsResponse
	{
		public readonly int robitsReward;

		public readonly int averageSeasonExperienceForEveryoneInClan;

		public readonly int clansTotalExperience;

		public readonly int seasonMonth;

		public readonly int seasonYear;

		public readonly bool hasClaimedAllRewards;

		public readonly string clanName;

		public readonly int totalSeasonXPForThisPlayer;

		public FetchSeasonRewardsResponse(int robitsReward_, bool hasClaimedAllRewards_, int seasonMonth_, int seasonYear_, int averageSeasonExperienceForEveryoneInClan_, int clansTotalExperience_, string clanname_, int totalSeasonXPForThisPlayer_)
		{
			robitsReward = robitsReward_;
			hasClaimedAllRewards = hasClaimedAllRewards_;
			seasonMonth = seasonMonth_;
			seasonYear = seasonYear_;
			averageSeasonExperienceForEveryoneInClan = averageSeasonExperienceForEveryoneInClan_;
			clansTotalExperience = clansTotalExperience_;
			clanName = clanname_;
			totalSeasonXPForThisPlayer = totalSeasonXPForThisPlayer_;
		}
	}
}
