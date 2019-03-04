using Simulation;
using Simulation.Hardware.Weapons;
using System.Collections.Generic;

internal sealed class NetworkMachineManager
{
	private Dictionary<TargetType, Dictionary<int, IMachineMap>> _machineMaps = new Dictionary<TargetType, Dictionary<int, IMachineMap>>(new TargetTypeComparer());

	public void RegisterMachineMap(TargetType type, int machineId, IMachineMap map)
	{
		if (!_machineMaps.ContainsKey(type))
		{
			_machineMaps[type] = new Dictionary<int, IMachineMap>();
		}
		_machineMaps[type][machineId] = map;
	}

	public void UnregisterMachineMap(TargetType type, int machineId)
	{
		_machineMaps[type].Remove(machineId);
	}

	public IMachineMap GetMachineMap(TargetType type, int machineId)
	{
		return _machineMaps[type][machineId];
	}

	public bool IsPlayerMapRegisterred(TargetType type, int machineId)
	{
		return _machineMaps[type].ContainsKey(machineId);
	}
}
