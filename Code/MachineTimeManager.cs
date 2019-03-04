using System.Collections.Generic;

internal sealed class MachineTimeManager
{
	private Dictionary<int, IMachineUpdater> _machineUpdaters = new Dictionary<int, IMachineUpdater>();

	public bool IsMachineRegistered(int player)
	{
		return _machineUpdaters.ContainsKey(player);
	}

	public void RegisterUpdater(int player, IMachineUpdater machineUpdater)
	{
		if (!_machineUpdaters.ContainsKey(player))
		{
			_machineUpdaters.Add(player, machineUpdater);
		}
		else
		{
			_machineUpdaters[player] = machineUpdater;
		}
	}

	public void UnregisterUpdater(int player)
	{
		if (_machineUpdaters.ContainsKey(player))
		{
			_machineUpdaters.Remove(player);
		}
	}

	public IMachineUpdater GetUpdaterForPlayer(int playerId)
	{
		if (_machineUpdaters.ContainsKey(playerId))
		{
			return _machineUpdaters[playerId];
		}
		return null;
	}
}
