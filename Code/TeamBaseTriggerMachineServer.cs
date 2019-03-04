using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class TeamBaseTriggerMachineServer
{
	private Dictionary<int, TeamBaseTrigger> _teamBaseTriggers = new Dictionary<int, TeamBaseTrigger>();

	[Inject]
	public CentreOfMassContainer centreOfMassContainer
	{
		private get;
		set;
	}

	[Inject]
	public MachineTransformContainer machineTransformContainer
	{
		private get;
		set;
	}

	public void AddTeamBaseTriggerObject(int owner, IEnumerable<CubeData> allCubes)
	{
		TeamBaseTrigger teamBaseTrigger = new TeamBaseTrigger();
		teamBaseTrigger.minMax = GridScaleUtility.GetMinAndMaxCubePos(allCubes, TargetType.Player);
		_teamBaseTriggers.Add(owner, teamBaseTrigger);
	}

	public bool IsActive(int player)
	{
		return _teamBaseTriggers.ContainsKey(player);
	}

	public bool IsMachineInRange(int playerId, Vector3 baseCentre, float baseRadiusSquared)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Vector3 machineWorldCOM = machineTransformContainer.GetMachineWorldCOM(TargetType.Player, playerId);
		Vector3 val = baseCentre - machineWorldCOM;
		float sqrMagnitude = val.get_sqrMagnitude();
		return sqrMagnitude <= baseRadiusSquared;
	}

	public bool IsMachinePositionValid(int playerId, Vector3 baseCentre)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 machineWorldCOM = machineTransformContainer.GetMachineWorldCOM(TargetType.Player, playerId);
		return machineWorldCOM.y > baseCentre.y + 1f;
	}

	public bool IsMachineCollidingWithSphere(int playerId, Vector3 baseCentre, float baseRadiusSquared)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localPosRelToPlayerMachine = TransformPositionToMachineSpace(playerId, baseCentre);
		Vector3[] machineSpaceMinMax = GetMachineSpaceMinMax(playerId);
		return IsMachineInRange(playerId, localPosRelToPlayerMachine, baseRadiusSquared, machineSpaceMinMax);
	}

	private Vector3 TransformPositionToMachineSpace(int player, Vector3 worldPos)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = machineTransformContainer.GetMachineWorldCOM(TargetType.Player, player) + machineTransformContainer.GetMachineRotation(TargetType.Player, player) * centreOfMassContainer.GetCentreOfMass(player);
		return Quaternion.Inverse(machineTransformContainer.GetMachineRotation(TargetType.Player, player)) * (worldPos - val);
	}

	private Vector3[] GetMachineSpaceMinMax(int player)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])new Vector3[2];
		Array.Copy(_teamBaseTriggers[player].minMax, array, 2);
		Vector3 centreOfMass = centreOfMassContainer.GetCentreOfMass(player);
		for (int i = 0; i < array.Length; i++)
		{
			ref Vector3 reference = ref array[i];
			reference -= centreOfMass;
		}
		return array;
	}

	private bool IsMachineInRange(int player, Vector3 localPosRelToPlayerMachine, float distSq, Vector3[] minMax)
	{
		float num = 0f;
		if (localPosRelToPlayerMachine.x < minMax[0].x)
		{
			num += Square(localPosRelToPlayerMachine.x - minMax[0].x);
		}
		else if (localPosRelToPlayerMachine.x > minMax[1].x)
		{
			num += Square(localPosRelToPlayerMachine.x - minMax[1].x);
		}
		if (localPosRelToPlayerMachine.y < minMax[0].y)
		{
			num += Square(localPosRelToPlayerMachine.y - minMax[0].y);
		}
		else if (localPosRelToPlayerMachine.y > minMax[1].y)
		{
			num += Square(localPosRelToPlayerMachine.y - minMax[1].y);
		}
		if (localPosRelToPlayerMachine.z < minMax[0].z)
		{
			num += Square(localPosRelToPlayerMachine.z - minMax[0].z);
		}
		else if (localPosRelToPlayerMachine.z > minMax[1].z)
		{
			num += Square(localPosRelToPlayerMachine.z - minMax[1].z);
		}
		return num <= distSq;
	}

	private float Square(float x)
	{
		return x * x;
	}

	public void OnMachineUnregistered(int owner)
	{
		DisableTeamBaseTriggerObject(owner);
	}

	private void DisableTeamBaseTriggerObject(int owner)
	{
		_teamBaseTriggers[owner].isActive = false;
	}

	private void RemoveTeamBaseTriggerObject(int owner)
	{
		_teamBaseTriggers.Remove(owner);
	}
}
