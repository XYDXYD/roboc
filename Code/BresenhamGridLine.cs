using Simulation;
using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using UnityEngine;

internal sealed class BresenhamGridLine
{
	public static bool GetFirstCubeOnLine(RaycastHit rcHit, Vector3 position, Vector3 direction, float weaponRange, NetworkMachineManager machineManager, MachineRootContainer machineRootContainer, TargetType targetType, Byte3? cubeToIgnore, ref Byte3 hitCube, ref int hitMachineId, ref IMachineMap hitMachineMap)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody machineRigidbody = GameUtility.GetMachineRigidbody(rcHit.get_collider().get_transform());
		if (machineRigidbody == null)
		{
			hitMachineId = -1;
			return false;
		}
		Vector3 normalized = direction.get_normalized();
		GameObject machineBoard = GameUtility.GetMachineBoard(machineRigidbody.get_transform());
		hitMachineId = machineRootContainer.GetMachineIdFromRoot(TargetType.Player, machineBoard);
		Quaternion val = Quaternion.Inverse(machineRigidbody.get_rotation());
		Vector3 pos = val * (normalized * 0.02f + rcHit.get_point() - machineRigidbody.get_position());
		Vector3 val2 = GridScaleUtility.InverseWorldScale(pos, targetType);
		Vector3 val3 = val * direction.get_normalized();
		hitMachineMap = machineManager.GetMachineMap(TargetType.Player, hitMachineId);
		float num = GridScaleUtility.InverseWorldScale(weaponRange - Vector3.Distance(position, rcHit.get_point()), targetType);
		Vector3 val4 = val2 + val3 * num;
		Int3 gridLoc = new Int3(Mathf.FloorToInt(val2.x), Mathf.FloorToInt(val2.y), Mathf.FloorToInt(val2.z));
		Int3 @int = new Int3(Mathf.FloorToInt(val4.x), Mathf.FloorToInt(val4.y), Mathf.FloorToInt(val4.z));
		int num2 = @int.x - gridLoc.x;
		int num3 = @int.y - gridLoc.y;
		int num4 = @int.z - gridLoc.z;
		int num5 = Mathf.Abs(num2);
		int num6 = Mathf.Abs(num3);
		int num7 = Mathf.Abs(num4);
		int num8 = num5 << 1;
		int num9 = num6 << 1;
		int num10 = num7 << 1;
		int num11 = (num2 >= 0) ? 1 : (-1);
		int num12 = (num3 >= 0) ? 1 : (-1);
		int num13 = (num4 >= 0) ? 1 : (-1);
		if (num5 > num6 && num5 > num7)
		{
			float num14 = num9 - num5;
			float num15 = num10 - num5;
			for (int i = 0; i < num5; i++)
			{
				int num16 = i;
				Byte3 @byte = hitMachineMap.GridSize();
				if (num16 >= @byte.x)
				{
					break;
				}
				MachineCell cellAt = hitMachineMap.GetCellAt(gridLoc);
				if (cellAt != null && cellAt.info != null)
				{
					hitCube = cellAt.pos;
					return true;
				}
				bool flag = false;
				if (num14 > 0f)
				{
					gridLoc.y += num12;
					num14 -= (float)num8;
				}
				if (num15 > 0f)
				{
					gridLoc.z += num13;
					num15 -= (float)num8;
				}
				if (flag)
				{
					cellAt = hitMachineMap.GetCellAt(gridLoc);
					if (cellAt != null && cellAt.info != null)
					{
						hitCube = cellAt.pos;
						return true;
					}
				}
				num14 += (float)num9;
				num15 += (float)num10;
				gridLoc.x += num11;
			}
		}
		else if (num6 > num5 && num6 > num7)
		{
			float num17 = num8 - num6;
			float num18 = num10 - num6;
			for (int j = 0; j < num6; j++)
			{
				int num19 = j;
				Byte3 byte2 = hitMachineMap.GridSize();
				if (num19 >= byte2.y)
				{
					break;
				}
				MachineCell cellAt2 = hitMachineMap.GetCellAt(gridLoc);
				if (cellAt2 != null && cellAt2.info != null)
				{
					hitCube = cellAt2.pos;
					return true;
				}
				bool flag2 = false;
				if (num17 > 0f)
				{
					gridLoc.x += num11;
					num17 -= (float)num9;
				}
				if (num18 > 0f)
				{
					gridLoc.z += num13;
					num18 -= (float)num9;
				}
				if (flag2)
				{
					cellAt2 = hitMachineMap.GetCellAt(gridLoc);
					if (cellAt2 != null && cellAt2.info != null)
					{
						hitCube = cellAt2.pos;
						return true;
					}
				}
				num17 += (float)num8;
				num18 += (float)num10;
				gridLoc.y += num12;
			}
		}
		else if (num7 > num5 && num7 > num6)
		{
			float num20 = num8 - num7;
			float num21 = num9 - num7;
			for (int k = 0; k < num7; k++)
			{
				int num22 = k;
				Byte3 byte3 = hitMachineMap.GridSize();
				if (num22 >= byte3.z)
				{
					break;
				}
				MachineCell cellAt3 = hitMachineMap.GetCellAt(gridLoc);
				if (cellAt3 != null && cellAt3.info != null)
				{
					hitCube = cellAt3.pos;
					return true;
				}
				bool flag3 = false;
				if (num20 > 0f)
				{
					gridLoc.x += num11;
					num20 -= (float)num10;
				}
				if (num21 > 0f)
				{
					gridLoc.y += num12;
					num21 -= (float)num10;
				}
				if (flag3)
				{
					cellAt3 = hitMachineMap.GetCellAt(gridLoc);
					if (cellAt3 != null && cellAt3.info != null)
					{
						hitCube = cellAt3.pos;
						return true;
					}
				}
				num20 += (float)num8;
				num21 += (float)num9;
				gridLoc.z += num13;
			}
		}
		return false;
	}

	public static void GetCubesOnLineForDamage(Byte3 hitCubePos, IMachineMap hitMachineMap, int damageToDeal, Vector3 gridDirection, float gridDistance, ref Dictionary<InstantiatedCube, int> proposedDamage)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		int totalDamage = 0;
		Vector3 val = hitCubePos.ToVector3() + gridDirection * gridDistance;
		Int3 @int = new Int3(Mathf.FloorToInt(val.x), Mathf.FloorToInt(val.y), Mathf.FloorToInt(val.z));
		Int3 gridLoc = new Int3(hitCubePos.x, hitCubePos.y, hitCubePos.z);
		int num = @int.x - gridLoc.x;
		int num2 = @int.y - gridLoc.y;
		int num3 = @int.z - gridLoc.z;
		int num4 = Mathf.Abs(num);
		int num5 = Mathf.Abs(num2);
		int num6 = Mathf.Abs(num3);
		int num7 = num4 << 1;
		int num8 = num5 << 1;
		int num9 = num6 << 1;
		int num10 = (num >= 0) ? 1 : (-1);
		int num11 = (num2 >= 0) ? 1 : (-1);
		int num12 = (num3 >= 0) ? 1 : (-1);
		if (num4 > num5 && num4 > num6)
		{
			float num13 = num8 - num4;
			float num14 = num9 - num4;
			Byte3 @byte = hitMachineMap.GridSize();
			int x = @byte.x;
			for (int i = 0; i < num4 && i < x; i++)
			{
				MachineCell cellAt = hitMachineMap.GetCellAt(gridLoc);
				if (cellAt != null && cellAt.info != null && !proposedDamage.ContainsKey(cellAt.info))
				{
					AddDamagedCube(cellAt.info, damageToDeal, ref totalDamage, ref proposedDamage);
					if (totalDamage == damageToDeal)
					{
						break;
					}
				}
				bool flag = false;
				if (num13 > 0f)
				{
					gridLoc.y += num11;
					num13 -= (float)num7;
				}
				if (num14 > 0f)
				{
					gridLoc.z += num12;
					num14 -= (float)num7;
				}
				if (flag)
				{
					cellAt = hitMachineMap.GetCellAt(gridLoc);
					if (cellAt != null && cellAt.info != null && !proposedDamage.ContainsKey(cellAt.info))
					{
						AddDamagedCube(cellAt.info, damageToDeal, ref totalDamage, ref proposedDamage);
						if (totalDamage == damageToDeal)
						{
							break;
						}
					}
				}
				num13 += (float)num8;
				num14 += (float)num9;
				gridLoc.x += num10;
			}
		}
		else if (num5 > num4 && num5 > num6)
		{
			float num15 = num7 - num5;
			float num16 = num9 - num5;
			Byte3 byte2 = hitMachineMap.GridSize();
			int y = byte2.y;
			for (int j = 0; j < num5 && j < y; j++)
			{
				MachineCell cellAt2 = hitMachineMap.GetCellAt(gridLoc);
				if (cellAt2 != null && cellAt2.info != null && !proposedDamage.ContainsKey(cellAt2.info))
				{
					AddDamagedCube(cellAt2.info, damageToDeal, ref totalDamage, ref proposedDamage);
					if (totalDamage == damageToDeal)
					{
						break;
					}
				}
				bool flag2 = false;
				if (num15 > 0f)
				{
					gridLoc.x += num10;
					num15 -= (float)num8;
				}
				if (num16 > 0f)
				{
					gridLoc.z += num12;
					num16 -= (float)num8;
				}
				if (flag2)
				{
					cellAt2 = hitMachineMap.GetCellAt(gridLoc);
					if (cellAt2 != null && cellAt2.info != null && !proposedDamage.ContainsKey(cellAt2.info))
					{
						AddDamagedCube(cellAt2.info, damageToDeal, ref totalDamage, ref proposedDamage);
						if (totalDamage == damageToDeal)
						{
							break;
						}
					}
				}
				num15 += (float)num7;
				num16 += (float)num9;
				gridLoc.y += num11;
			}
		}
		else
		{
			if (num6 <= num4 || num6 <= num5)
			{
				return;
			}
			float num17 = num7 - num6;
			float num18 = num8 - num6;
			Byte3 byte3 = hitMachineMap.GridSize();
			int z = byte3.z;
			for (int k = 0; k < num6 && k < z; k++)
			{
				MachineCell cellAt3 = hitMachineMap.GetCellAt(gridLoc);
				if (cellAt3 != null && cellAt3.info != null && !proposedDamage.ContainsKey(cellAt3.info))
				{
					AddDamagedCube(cellAt3.info, damageToDeal, ref totalDamage, ref proposedDamage);
					if (totalDamage == damageToDeal)
					{
						break;
					}
				}
				bool flag3 = false;
				if (num17 > 0f)
				{
					gridLoc.x += num10;
					num17 -= (float)num9;
				}
				if (num18 > 0f)
				{
					gridLoc.y += num11;
					num18 -= (float)num9;
				}
				if (flag3)
				{
					cellAt3 = hitMachineMap.GetCellAt(gridLoc);
					if (cellAt3 != null && cellAt3.info != null && !proposedDamage.ContainsKey(cellAt3.info))
					{
						AddDamagedCube(cellAt3.info, damageToDeal, ref totalDamage, ref proposedDamage);
						if (totalDamage == damageToDeal)
						{
							break;
						}
					}
				}
				num17 += (float)num7;
				num18 += (float)num8;
				gridLoc.z += num12;
			}
		}
	}

	private static void AddDamagedCube(InstantiatedCube cubeInstance, int damage, ref int totalDamage, ref Dictionary<InstantiatedCube, int> destroyedTargets)
	{
		int health = cubeInstance.health;
		int num = Mathf.Min(health, damage - totalDamage);
		totalDamage += num;
		destroyedTargets.Add(cubeInstance, health - num);
	}
}
