using Simulation;
using Svelto.DataStructures;
using System;
using UnityEngine;

internal static class MachineUtility
{
	public static Int3 GetDirection(CubeFace direction)
	{
		switch (direction)
		{
		case CubeFace.Up:
			return Int3.up;
		case CubeFace.Down:
			return Int3.down;
		case CubeFace.Front:
			return Int3.forward;
		case CubeFace.Back:
			return Int3.back;
		case CubeFace.Right:
			return Int3.right;
		case CubeFace.Left:
			return Int3.left;
		default:
			throw new Exception("GetDirection invalid direction");
		}
	}

	public static CubeFace FindWorldDirection(Vector3 direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normalized = direction.get_normalized();
		float num = Vector3.Dot(normalized, Vector3.get_up());
		if (num > 0.95f)
		{
			return CubeFace.Up;
		}
		if (num < -0.95f)
		{
			return CubeFace.Down;
		}
		num = Vector3.Dot(normalized, Vector3.get_forward());
		if (num > 0.95f)
		{
			return CubeFace.Front;
		}
		if (num < -0.95f)
		{
			return CubeFace.Back;
		}
		num = Vector3.Dot(normalized, Vector3.get_right());
		if (num > 0.95f)
		{
			return CubeFace.Right;
		}
		if (num < -0.95f)
		{
			return CubeFace.Left;
		}
		throw new Exception("FindWorldDirection invalid direction");
	}

	public static CubeFace FindWorldDirection(Int3 direction)
	{
		if (direction == Int3.back)
		{
			return CubeFace.Back;
		}
		if (direction == Int3.forward)
		{
			return CubeFace.Front;
		}
		if (direction == Int3.left)
		{
			return CubeFace.Left;
		}
		if (direction == Int3.right)
		{
			return CubeFace.Right;
		}
		if (direction == Int3.up)
		{
			return CubeFace.Up;
		}
		if (direction == Int3.down)
		{
			return CubeFace.Down;
		}
		throw new Exception("FindWorldDirection invalid direction");
	}

	public static CubeFace GetOffsetFromDirection(Vector3 direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normalized = direction.get_normalized();
		float num = Vector3.Dot(normalized, Vector3.get_up());
		if (num > 0.95f)
		{
			return CubeFace.Up;
		}
		if (num < -0.95f)
		{
			return CubeFace.Down;
		}
		num = Vector3.Dot(normalized, Vector3.get_forward());
		if (num > 0.95f)
		{
			return CubeFace.Front;
		}
		if (num < -0.95f)
		{
			return CubeFace.Back;
		}
		num = Vector3.Dot(normalized, Vector3.get_right());
		if (num > 0.95f)
		{
			return CubeFace.Right;
		}
		if (num < -0.95f)
		{
			return CubeFace.Left;
		}
		return CubeFace.Other;
	}

	public static Vector3 MachineCenterWorld(out Vector3 min, FasterList<Transform> cubes)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] minAndMaxCubePos = GetMinAndMaxCubePos(cubes);
		min = minAndMaxCubePos[0];
		return (min + minAndMaxCubePos[1] + Vector3.get_one()) * 0.5f;
	}

	public static Vector3[] GetMinAndMaxCubePos(Transform[] cubes)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_zero();
		Vector3 val2 = Vector3.get_zero();
		if (cubes.Length > 0)
		{
			val = (val2 = cubes[0].get_position());
		}
		foreach (Transform val3 in cubes)
		{
			for (int j = 0; j < 3; j++)
			{
				Vector3 position = val3.get_position();
				if (position.get_Item(j) < val.get_Item(j))
				{
					int num = j;
					Vector3 position2 = val3.get_position();
					val.set_Item(num, position2.get_Item(j));
				}
				Vector3 position3 = val3.get_position();
				if (position3.get_Item(j) > val2.get_Item(j))
				{
					int num2 = j;
					Vector3 position4 = val3.get_position();
					val2.set_Item(num2, position4.get_Item(j));
				}
			}
		}
		return (Vector3[])new Vector3[2]
		{
			val,
			val2
		};
	}

	public static Vector3[] GetMinAndMaxCubePos(FasterList<Transform> cubes)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.get_zero();
		Vector3 val2 = Vector3.get_zero();
		if (cubes.get_Count() > 0)
		{
			val = (val2 = cubes.get_Item(0).get_position());
		}
		FasterListEnumerator<Transform> enumerator = cubes.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform current = enumerator.get_Current();
				for (int i = 0; i < 3; i++)
				{
					Vector3 position = current.get_position();
					if (position.get_Item(i) < val.get_Item(i))
					{
						int num = i;
						Vector3 position2 = current.get_position();
						val.set_Item(num, position2.get_Item(i));
					}
					Vector3 position3 = current.get_position();
					if (position3.get_Item(i) > val2.get_Item(i))
					{
						int num2 = i;
						Vector3 position4 = current.get_position();
						val2.set_Item(num2, position4.get_Item(i));
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return (Vector3[])new Vector3[2]
		{
			val,
			val2
		};
	}

	public static int SortByPosition(Transform a, Transform b)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = a.get_position();
		Vector3 position2 = b.get_position();
		for (int i = 0; i < 3; i++)
		{
			if (position.get_Item(i) < position2.get_Item(i))
			{
				return -1;
			}
			if (position.get_Item(i) > position2.get_Item(i))
			{
				return 1;
			}
		}
		return 0;
	}

	public static int SortObjectByPosition<T>(T a, T b) where T : Component
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = a.get_transform().get_position();
		Vector3 position2 = b.get_transform().get_position();
		for (int i = 0; i < 3; i++)
		{
			if (position.get_Item(i) < position2.get_Item(i))
			{
				return -1;
			}
			if (position.get_Item(i) > position2.get_Item(i))
			{
				return 1;
			}
		}
		return 0;
	}

	public static int SortByRaycastDistance(RaycastHit a, RaycastHit b)
	{
		if (a.get_distance() < b.get_distance())
		{
			return -1;
		}
		if (a.get_distance() > b.get_distance())
		{
			return 1;
		}
		return 0;
	}

	internal static InstantiatedCube GetCubeAtGridPosition(Byte3 hitGridPos, IMachineMap machineMap)
	{
		return machineMap.GetCellAt(hitGridPos).info;
	}

	public static Byte3 GetAdjacentCell(CubeFace face, int rotationIndex, Byte3 gridPos)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GetDirection(face).ToVector3();
		Vector3 vec = CubeData.IndexToQuat(rotationIndex) * val;
		return gridPos + new Byte3(vec);
	}
}
