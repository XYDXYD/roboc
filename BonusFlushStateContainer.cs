using System.Collections.Generic;

internal sealed class BonusFlushStateContainer
{
	private HashSet<int> _flushedPlayers = new HashSet<int>();

	public void PlayerFlushed(int playerId)
	{
		_flushedPlayers.Add(playerId);
	}

	public bool HasPlayerBeenFlushed(int playerId)
	{
		return _flushedPlayers.Contains(playerId);
	}
}
