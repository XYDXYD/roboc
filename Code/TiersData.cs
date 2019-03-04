internal class TiersData
{
	internal readonly uint[] TiersBands;

	internal readonly uint MegabotTierIndex;

	internal readonly uint MaxRobotRankingARobotCanObtain;

	internal TiersData(uint[] tiersBands, uint maxRobotRankingARobotCanObtain)
	{
		TiersBands = tiersBands;
		MegabotTierIndex = (uint)TiersBands.Length;
		MaxRobotRankingARobotCanObtain = maxRobotRankingARobotCanObtain;
	}
}
