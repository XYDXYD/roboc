using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using UnityEngine;

internal sealed class MachineTransformContainer
{
	private Dictionary<TargetType, Dictionary<int, Vector3>> _machinePositions = new Dictionary<TargetType, Dictionary<int, Vector3>>(new TargetTypeComparer());

	private Dictionary<TargetType, Dictionary<int, Quaternion>> _machineRotations = new Dictionary<TargetType, Dictionary<int, Quaternion>>(new TargetTypeComparer());

	public void SetMachineWorldCOM(TargetType type, int player, Vector3 position)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!_machinePositions.ContainsKey(type))
		{
			_machinePositions[type] = new Dictionary<int, Vector3>();
		}
		_machinePositions[type][player] = position;
	}

	public Vector3 GetMachineWorldCOM(TargetType type, int player)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return _machinePositions[type][player];
	}

	public void SetMachineRotation(TargetType type, int player, Quaternion rotation)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!_machineRotations.ContainsKey(type))
		{
			_machineRotations[type] = new Dictionary<int, Quaternion>();
		}
		_machineRotations[type][player] = rotation;
	}

	public Quaternion GetMachineRotation(TargetType type, int player)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return _machineRotations[type][player];
	}
}
