using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using UnityEngine;

internal sealed class RigidbodyDataContainer
{
	private Dictionary<TargetType, Dictionary<int, Rigidbody>> _rigidBodyData = new Dictionary<TargetType, Dictionary<int, Rigidbody>>(new TargetTypeComparer());

	public void RegisterRigidBodyData(TargetType type, int machineId, Rigidbody rbData)
	{
		if (!_rigidBodyData.ContainsKey(type))
		{
			_rigidBodyData[type] = new Dictionary<int, Rigidbody>();
		}
		_rigidBodyData[type][machineId] = rbData;
	}

	public void UnregisterRigidBodyData(TargetType type, int machineId)
	{
		_rigidBodyData[type].Remove(machineId);
	}

	public Rigidbody GetRigidBodyData(TargetType type, int machineId)
	{
		return _rigidBodyData[type][machineId];
	}
}
