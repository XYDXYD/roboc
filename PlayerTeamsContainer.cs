using Simulation.Hardware.Weapons;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

internal sealed class PlayerTeamsContainer
{
	public const int NEUTRAL_TEAM = -1;

	private bool _setLocalId;

	private int _localPlayerId;

	private Dictionary<TargetType, Dictionary<int, int>> _playerTeams = new Dictionary<TargetType, Dictionary<int, int>>(new TargetTypeComparer());

	private Dictionary<TargetType, Dictionary<int, List<int>>> _teamPlayers = new Dictionary<TargetType, Dictionary<int, List<int>>>(new TargetTypeComparer());

	private Dictionary<TargetType, Dictionary<int, List<int>>> _originalTeamPlayers = new Dictionary<TargetType, Dictionary<int, List<int>>>(new TargetTypeComparer());

	private Dictionary<TargetType, List<int>> _enemies = new Dictionary<TargetType, List<int>>(new TargetTypeComparer());

	private Action _onLoaded;

	internal int localPlayerId => _localPlayerId;

	public bool OwnIdIsRegistered()
	{
		return _setLocalId;
	}

	public bool IsSet()
	{
		return _teamPlayers.Count != 0 && _playerTeams.Count != 0;
	}

	public void SetLocalId(int playerId)
	{
		_setLocalId = true;
		_localPlayerId = playerId;
	}

	public void RemoveLocalId()
	{
		_setLocalId = false;
		_localPlayerId = 0;
	}

	public bool IsMe(TargetType type, int playerId)
	{
		if (type == TargetType.Player && OwnIdIsRegistered())
		{
			return playerId == localPlayerId;
		}
		return false;
	}

	public void RegisterPlayerTeam(TargetType type, int player, int team)
	{
		if (!_playerTeams.ContainsKey(type))
		{
			_playerTeams[type] = new Dictionary<int, int>();
			_teamPlayers[type] = new Dictionary<int, List<int>>();
			_originalTeamPlayers[type] = new Dictionary<int, List<int>>();
			_enemies[type] = new List<int>();
		}
		_playerTeams[type].Add(player, team);
		if (!_teamPlayers[type].ContainsKey(team))
		{
			_teamPlayers[type].Add(team, new List<int>());
			_originalTeamPlayers[type].Add(team, new List<int>());
		}
		_teamPlayers[type][team].Add(player);
		_originalTeamPlayers[type][team].Add(player);
		RefreshEnemies(type);
		if (OwnIdIsRegistered() && _onLoaded != null)
		{
			_onLoaded();
			_onLoaded = null;
		}
	}

	public void ChangePlayerTeam(TargetType type, int player, int team)
	{
		if (_playerTeams[type].ContainsKey(player))
		{
			int key = _playerTeams[type][player];
			if (_teamPlayers[type].ContainsKey(key))
			{
				_teamPlayers[type][key].Remove(player);
			}
		}
		if (!_teamPlayers[type].ContainsKey(team))
		{
			_teamPlayers[type][team] = new List<int>();
		}
		_teamPlayers[type][team].Add(player);
		_playerTeams[type][player] = team;
		RefreshEnemies(type);
	}

	public void UnregisterPlayerTeam(TargetType type, int player)
	{
		_teamPlayers[type][_playerTeams[type][player]].Remove(player);
		_playerTeams[type].Remove(player);
		RefreshEnemies(type);
	}

	public int GetPlayerTeam(TargetType type, int player)
	{
		return _playerTeams[type][player];
	}

	public int GetPlayerOriginalTeam(TargetType type, int player)
	{
		int result = -1;
		using (Dictionary<int, List<int>>.Enumerator enumerator = _originalTeamPlayers[type].GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Value.Contains(player))
				{
					return enumerator.Current.Key;
				}
			}
			return result;
		}
	}

	public int GetEnemyTeam(TargetType type, int player)
	{
		int num = -1;
		Dictionary<int, int>.Enumerator enumerator = _playerTeams[type].GetEnumerator();
		while (enumerator.MoveNext() && num == -1)
		{
			if (_playerTeams[type][player] != enumerator.Current.Value)
			{
				num = enumerator.Current.Value;
			}
		}
		return num;
	}

	public bool IsOnMyTeam(TargetType type, int player)
	{
		if (!OwnIdIsRegistered())
		{
			return false;
		}
		return _playerTeams[type][player] == _playerTeams[TargetType.Player][localPlayerId];
	}

	public bool IsMyTeam(int team)
	{
		if (!OwnIdIsRegistered())
		{
			return false;
		}
		return _playerTeams[TargetType.Player][localPlayerId] == team;
	}

	public List<int> GetPlayersOnEnemyTeam(TargetType type)
	{
		if (!_setLocalId || _enemies[type].Count > 0)
		{
			return _enemies[type];
		}
		RefreshEnemies(type);
		return _enemies[type];
	}

	public int GetMyTeam()
	{
		return _playerTeams[TargetType.Player][localPlayerId];
	}

	public int[] GetMyEnemyTeam()
	{
		List<int> list = new List<int>();
		int num = _playerTeams[TargetType.Player][localPlayerId];
		Dictionary<int, int>.Enumerator enumerator = _playerTeams[TargetType.Player].GetEnumerator();
		while (enumerator.MoveNext())
		{
			int value = enumerator.Current.Value;
			if (value != num)
			{
				list.Add(value);
			}
		}
		return list.ToArray();
	}

	public ReadOnlyCollection<int> GetPlayersOnTeam(TargetType type, int team)
	{
		return _teamPlayers[type][team].AsReadOnly();
	}

	public int GetPlayersCountOnTeam(TargetType type, int team)
	{
		if (_teamPlayers.ContainsKey(type) && _teamPlayers[type].ContainsKey(team))
		{
			return _teamPlayers[type][team].Count;
		}
		return 0;
	}

	public bool IsOnlyOneTeamRemaining(TargetType type, out int remainingTeam)
	{
		int num = 0;
		remainingTeam = -1;
		foreach (KeyValuePair<int, List<int>> item in _teamPlayers[type])
		{
			if (item.Value.Count > 0)
			{
				num++;
				remainingTeam = item.Key;
				if (num > 1)
				{
					return false;
				}
			}
		}
		if (num == 1)
		{
			return true;
		}
		return false;
	}

	public int GetNumberDeregisteredPlayers(TargetType type, int teamId)
	{
		int count = _teamPlayers[type][teamId].Count;
		int count2 = _originalTeamPlayers[type][teamId].Count;
		return count2 - count;
	}

	public void IfOrWhenOwnIdRegistered(Action callback)
	{
		if (OwnIdIsRegistered())
		{
			callback();
		}
		else
		{
			_onLoaded = (Action)Delegate.Combine(_onLoaded, callback);
		}
	}

	private void RefreshEnemies(TargetType type)
	{
		if (_setLocalId)
		{
			_enemies[type].Clear();
			int num = _playerTeams[TargetType.Player][localPlayerId];
			foreach (KeyValuePair<int, List<int>> item in _teamPlayers[type])
			{
				if (item.Key != num)
				{
					_enemies[type].AddRange(item.Value);
				}
			}
		}
	}

	public Dictionary<int, List<int>> GetAllOriginalTeams(TargetType type)
	{
		return _originalTeamPlayers[type];
	}
}
