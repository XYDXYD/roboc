using System.Collections.Generic;

internal class InGamePlayerStats
{
	public int playerId
	{
		get;
		private set;
	}

	public List<InGameStat> inGameStats
	{
		get;
		private set;
	}

	public InGamePlayerStats(int _playerName, List<InGameStat> _inGameStats)
	{
		playerId = _playerName;
		inGameStats = _inGameStats;
	}
}
