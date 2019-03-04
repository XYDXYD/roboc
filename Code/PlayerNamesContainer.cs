using Svelto.DataStructures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

internal sealed class PlayerNamesContainer
{
	public class Player
	{
		public string name;

		public string displayName;

		public Player(string name, string displayName)
		{
			this.name = name;
			this.displayName = displayName;
		}
	}

	private bool _dataIsLoaded;

	private Dictionary<int, Player> _idToPlayer = new Dictionary<int, Player>();

	public bool IsNameRegistered(string name)
	{
		foreach (KeyValuePair<int, Player> item in _idToPlayer)
		{
			if (item.Value.name.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public void RegisterPlayerName(int player, string name, string displayName)
	{
		Player value = new Player(name, displayName);
		_idToPlayer[player] = value;
	}

	public void UnregisterPlayerName(int player)
	{
		Player player2 = _idToPlayer[player];
		_idToPlayer.Remove(player);
	}

	public bool TryGetPlayerName(int playerId, out string playerName)
	{
		Player value = null;
		playerName = null;
		if (_idToPlayer.TryGetValue(playerId, out value))
		{
			playerName = value.name;
			return true;
		}
		return false;
	}

	public bool TryGetDisplayName(int playerId, out string displayName)
	{
		Player value = null;
		displayName = null;
		if (_idToPlayer.TryGetValue(playerId, out value))
		{
			displayName = value.displayName;
			return true;
		}
		return false;
	}

	public string GetPlayerName(int player)
	{
		return _idToPlayer[player].name;
	}

	public string GetDisplayName(int player)
	{
		return _idToPlayer[player].displayName;
	}

	public int GetPlayerId(string name)
	{
		return _idToPlayer.FirstOrDefault((KeyValuePair<int, Player> x) => x.Value.name == name).Key;
	}

	public ReadOnlyDictionary<int, Player> GetIdToPlayer()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return new ReadOnlyDictionary<int, Player>(_idToPlayer);
	}

	public void OnPlayerIDsReceived()
	{
		_dataIsLoaded = true;
	}

	public IEnumerator LoadData()
	{
		while (!_dataIsLoaded)
		{
			yield return null;
		}
	}
}
