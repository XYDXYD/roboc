using Simulation.Hardware.Weapons;
using System.Collections.Generic;

internal sealed class LivePlayersContainer
{
	private List<int> _list = new List<int>();

	private Dictionary<TargetType, HashSet<int>> _livePlayers = new Dictionary<TargetType, HashSet<int>>(new TargetTypeComparer());

	public void MarkAsLive(TargetType type, int player)
	{
		if (!_livePlayers.ContainsKey(type))
		{
			_livePlayers[type] = new HashSet<int>();
		}
		_livePlayers[type].Add(player);
	}

	public void MarkAsDead(TargetType type, int player)
	{
		if (_livePlayers.ContainsKey(type))
		{
			_livePlayers[type].Remove(player);
		}
	}

	public bool IsPlayerAlive(TargetType type, int player)
	{
		if (!_livePlayers.ContainsKey(type))
		{
			return false;
		}
		return _livePlayers[type].Contains(player);
	}

	public int GetLivePlayersCount(TargetType type)
	{
		if (_livePlayers.ContainsKey(type))
		{
			return _livePlayers[type].Count;
		}
		return 0;
	}

	public List<int> GetLivePlayers(TargetType type)
	{
		_list.Clear();
		if (_livePlayers.ContainsKey(type))
		{
			_list.AddRange(_livePlayers[type]);
			return _list;
		}
		return _list;
	}
}
