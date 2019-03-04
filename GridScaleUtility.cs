using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using UnityEngine;

internal static class GridScaleUtility
{
	public static void MinMaxBoundsToGrid(Bounds b, TargetType targetType, out Int3 min, out Int3 max)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		float num = machineScaleData.halfCell * 0.9f;
		min = WorldToGrid(b.get_min() + Vector3.get_one() * num, targetType);
		max = WorldToGrid(b.get_max() - Vector3.get_one() * num, targetType);
	}

	public static Int3 WorldToGrid(Vector3 pos, TargetType targetType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		Vector3 val = pos * machineScaleData.invLevelScale;
		int x = Mathf.FloorToInt(val.x);
		int y = Mathf.FloorToInt(val.y);
		int z = Mathf.FloorToInt(val.z);
		return new Int3(x, y, z);
	}

	public static Byte3 WorldToGridByte3(Vector3 pos, TargetType targetType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		Vector3 val = pos * machineScaleData.invLevelScale;
		byte x = (byte)val.x;
		byte y = (byte)val.y;
		byte z = (byte)val.z;
		return new Byte3(x, y, z);
	}

	public static Vector3 GridToWorld(Byte3 pos, TargetType targetType)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		float levelScale = machineScaleData.levelScale;
		MachineScale.MachineScaleData machineScaleData2 = MachineScale.Scales[targetType];
		float halfCell = machineScaleData2.halfCell;
		return new Vector3((float)(int)pos.x * levelScale + halfCell, (float)(int)pos.y * levelScale + halfCell, (float)(int)pos.z * levelScale + halfCell);
	}

	public static Vector3 GridToWorld(Int3 pos, TargetType targetType)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		float levelScale = machineScaleData.levelScale;
		MachineScale.MachineScaleData machineScaleData2 = MachineScale.Scales[targetType];
		float halfCell = machineScaleData2.halfCell;
		return new Vector3((float)pos.x * levelScale + halfCell, (float)pos.y * levelScale + halfCell, (float)pos.z * levelScale + halfCell);
	}

	public static Vector3 GetMachineCenterInWorld(Byte3[] dimensions, TargetType targetType)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GridToWorld(dimensions[0], targetType);
		Vector3 val2 = GridToWorld(dimensions[1] + new Byte3(1, 1, 1), targetType);
		return (val2 + val) * 0.5f;
	}

	public static float WorldScale(float val, TargetType targetType)
	{
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		float levelScale = machineScaleData.levelScale;
		return val * levelScale;
	}

	public static Vector3 InverseWorldScale(Vector3 pos, TargetType targetType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		return pos * machineScaleData.invLevelScale;
	}

	public static float InverseWorldScale(float val, TargetType targetType)
	{
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		return val * machineScaleData.invLevelScale;
	}

	public static Vector3 WorldScale(Int3 pos, TargetType targetType)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		float levelScale = machineScaleData.levelScale;
		Vector3 result = default(Vector3);
		result._002Ector((float)pos.x * levelScale, (float)pos.y * levelScale, (float)pos.z * levelScale);
		return result;
	}

	public static Vector3 WorldScale(Byte3 pos, TargetType targetType)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		float levelScale = machineScaleData.levelScale;
		Vector3 result = default(Vector3);
		result._002Ector((float)(int)pos.x * levelScale, (float)(int)pos.y * levelScale, (float)(int)pos.z * levelScale);
		return result;
	}

	public static Vector3 WorldScale(Vector3 pos, TargetType targetType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		MachineScale.MachineScaleData machineScaleData = MachineScale.Scales[targetType];
		return pos * machineScaleData.levelScale;
	}

	public static Vector3 GetCubeWorldPosition(Byte3 gridPos, Rigidbody rb, TargetType targetType)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = rb.get_transform();
		return GetCubeWorldPosition(transform, gridPos, targetType);
	}

	public static Vector3 GetCubeWorldPosition(Transform root, Byte3 gridPos, TargetType targetType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		return root.get_rotation() * WorldScale(gridPos, targetType) + WorldScale(Vector3.get_one() * 0.5f, targetType) + root.get_position();
	}

	public static Vector3[] GetMinAndMaxCubePos(IEnumerable<InstantiatedCube> cubes, TargetType targetType)
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Byte3 pos = new Byte3(byte.MaxValue, byte.MaxValue, byte.MaxValue);
		Byte3 pos2 = new Byte3(0, 0, 0);
		foreach (InstantiatedCube cube in cubes)
		{
			for (int i = 0; i < 3; i++)
			{
				if (cube.gridPos[i] < pos[i])
				{
					pos[i] = cube.gridPos[i];
				}
				if (cube.gridPos[i] > pos2[i])
				{
					pos2[i] = cube.gridPos[i];
				}
			}
		}
		return (Vector3[])new Vector3[2]
		{
			WorldScale(pos, targetType),
			WorldScale(pos2, targetType)
		};
	}

	public static Vector3[] GetMinAndMaxCubePos(IEnumerable<CubeData> cubes, TargetType targetType)
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Byte3 pos = new Byte3(byte.MaxValue, byte.MaxValue, byte.MaxValue);
		Byte3 pos2 = new Byte3(0, 0, 0);
		foreach (CubeData cube in cubes)
		{
			for (int i = 0; i < 3; i++)
			{
				if (cube.gridLocation[i] < pos[i])
				{
					pos[i] = cube.gridLocation[i];
				}
				if (cube.gridLocation[i] > pos2[i])
				{
					pos2[i] = cube.gridLocation[i];
				}
			}
		}
		return (Vector3[])new Vector3[2]
		{
			WorldScale(pos, targetType),
			WorldScale(pos2, targetType)
		};
	}
}
