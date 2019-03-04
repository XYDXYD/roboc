using System.Collections.Generic;

internal sealed class PlayerKillsContainer
{
	private Dictionary<int, int> _playerKills = new Dictionary<int, int>();

	public void RegisterPlayerKills(int player, int kills)
	{
		_playerKills.Add(player, kills);
	}

	public void UnregisterPlayerKills(int player)
	{
		_playerKills.Remove(player);
	}

	public void AddPlayerKill(int shooter)
	{
		Dictionary<int, int> playerKills;
		int key;
		(playerKills = _playerKills)[key = shooter] = playerKills[key] + 1;
	}

	public int GetPlayerKills(int player)
	{
		return _playerKills[player];
	}
}
