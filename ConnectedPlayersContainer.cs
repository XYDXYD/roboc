using System;
using System.Collections.Generic;

internal sealed class ConnectedPlayersContainer
{
	public Action<int> OnPlayerDisconnected = delegate
	{
	};

	private HashSet<int> _connectedPlayerIds = new HashSet<int>();

	private Dictionary<string, bool> _currentBattleTotalCompletedPlayerNames = new Dictionary<string, bool>();

	public void PlayerConnected(int playerId, string playerName)
	{
		_connectedPlayerIds.Add(playerId);
		_currentBattleTotalCompletedPlayerNames[playerName] = false;
	}

	public void PlayerDisconnected(int playerId, string playerName)
	{
		_connectedPlayerIds.Remove(playerId);
		_currentBattleTotalCompletedPlayerNames.Remove(playerName);
		OnPlayerDisconnected(playerId);
	}

	public bool IsPlayerConnected(int playerId)
	{
		return _connectedPlayerIds.Contains(playerId);
	}

	public ICollection<int> GetConnectedPlayerIds()
	{
		return _connectedPlayerIds;
	}

	public Dictionary<string, bool> GetCurrentBattlesCompletedPlayerNames()
	{
		return _currentBattleTotalCompletedPlayerNames;
	}
}
