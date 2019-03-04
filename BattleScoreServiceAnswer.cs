internal struct BattleScoreServiceAnswer
{
	public uint lastBattleScore;

	public uint newTotalScore;

	public uint oldTotalScore;

	public uint newRange;

	public uint newRankInRange;

	public uint oldRange;

	public uint oldRankInRange;

	public BattleScoreServiceAnswer(BattleScoreServiceAnswer other)
	{
		lastBattleScore = other.lastBattleScore;
		newTotalScore = other.lastBattleScore;
		oldTotalScore = other.oldTotalScore;
		newRange = other.newRange;
		newRankInRange = other.newRankInRange;
		oldRange = other.oldRange;
		oldRankInRange = other.oldRankInRange;
	}

	public void Clean()
	{
		lastBattleScore = 0u;
		newTotalScore = 0u;
		oldTotalScore = 0u;
		newRange = 0u;
		newRankInRange = 0u;
		oldRange = 0u;
		oldRankInRange = 0u;
	}
}
