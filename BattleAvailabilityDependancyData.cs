internal sealed class BattleAvailabilityDependancyData
{
	public uint maxLevelForLeague
	{
		get;
		private set;
	}

	public int playerLevel
	{
		get;
		private set;
	}

	public BattleAvailabilityDependancyData(int playerLevel_, uint maxLevelForLeague_)
	{
		maxLevelForLeague = maxLevelForLeague_;
		playerLevel = playerLevel_;
	}
}
