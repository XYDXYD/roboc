using Simulation;
using Simulation.Hardware.Weapons;
using System.Collections.Generic;

internal sealed class MachineClusterContainer
{
	private Dictionary<TargetType, Dictionary<int, MachineCluster>> _machineClusters = new Dictionary<TargetType, Dictionary<int, MachineCluster>>(new TargetTypeComparer());

	private Dictionary<TargetType, Dictionary<int, MicrobotCollisionSphere>> _microbotCollisionSpheres = new Dictionary<TargetType, Dictionary<int, MicrobotCollisionSphere>>(new TargetTypeComparer());

	public void RegisterMachineCluster(TargetType type, int machineId, MachineCluster machine)
	{
		if (!_machineClusters.ContainsKey(type))
		{
			_machineClusters[type] = new Dictionary<int, MachineCluster>();
		}
		_machineClusters[type].Add(machineId, machine);
	}

	public void UnregisterMachineCluster(TargetType type, int machineId)
	{
		_machineClusters[type].Remove(machineId);
	}

	public MachineCluster GetMachineCluster(TargetType type, int machineId)
	{
		return _machineClusters[type][machineId];
	}

	public void RegisterMicrobotCollisionSphere(TargetType type, int machineId, MicrobotCollisionSphere machine)
	{
		if (!_microbotCollisionSpheres.ContainsKey(type))
		{
			_microbotCollisionSpheres[type] = new Dictionary<int, MicrobotCollisionSphere>();
		}
		_microbotCollisionSpheres[type].Add(machineId, machine);
	}

	public void UnregisterMicrobotCollisionSphere(TargetType type, int machineId)
	{
		_microbotCollisionSpheres[type].Remove(machineId);
	}

	public MicrobotCollisionSphere GetMicrobotCollisionSphere(TargetType type, int machineId)
	{
		return _microbotCollisionSpheres[type][machineId];
	}
}
