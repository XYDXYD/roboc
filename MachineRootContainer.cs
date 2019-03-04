using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using UnityEngine;

internal sealed class MachineRootContainer
{
	private Dictionary<TargetType, Dictionary<int, GameObject>> _machineRoots = new Dictionary<TargetType, Dictionary<int, GameObject>>(new TargetTypeComparer());

	private Dictionary<TargetType, Dictionary<GameObject, int>> _inverseRoots = new Dictionary<TargetType, Dictionary<GameObject, int>>(new TargetTypeComparer());

	public void RegisterMachineRoot(TargetType type, int machineId, GameObject machineObj)
	{
		if (!_machineRoots.ContainsKey(type))
		{
			_machineRoots[type] = new Dictionary<int, GameObject>();
			_inverseRoots[type] = new Dictionary<GameObject, int>();
		}
		_machineRoots[type][machineId] = machineObj;
		_inverseRoots[type][machineObj] = machineId;
	}

	public void UnregisterMachineRoot(TargetType type, int machineId)
	{
		_inverseRoots[type].Remove(_machineRoots[type][machineId]);
		_machineRoots[type].Remove(machineId);
	}

	public bool IsMachineRegistered(TargetType type, GameObject root)
	{
		if (!_machineRoots.ContainsKey(type))
		{
			return false;
		}
		return _machineRoots[type].ContainsValue(root);
	}

	public bool IsMachineRegistered(TargetType type, int machineId)
	{
		if (!_inverseRoots.ContainsKey(type))
		{
			return false;
		}
		return _inverseRoots[type].ContainsValue(machineId);
	}

	public GameObject GetMachineRoot(TargetType type, int machineId)
	{
		return _machineRoots[type][machineId];
	}

	public int GetMachineIdFromRoot(TargetType type, GameObject machineObj)
	{
		return _inverseRoots[type][machineObj];
	}

	public bool IsMachineRootRegistered(TargetType type, GameObject machineObj)
	{
		if (!_inverseRoots.ContainsKey(type))
		{
			return false;
		}
		return _inverseRoots[type].ContainsKey(machineObj);
	}
}
