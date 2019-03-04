using System;
using UnityEngine;

internal static class VoxelRaycast
{
	public struct Hit
	{
		public Int3 pos;

		public Int3 prevPos;
	}

	private const float g_infinite = 99999f;

	private static Hit _dummy = default(Hit);

	public static bool Cast(Ray ray, Predicate<Int3> hitFunc, float maxDistance = 100f)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		_dummy = default(Hit);
		return Cast(ray, hitFunc, out _dummy, maxDistance);
	}

	public static bool Cast(Ray ray, Predicate<Int3> hitFunc, out Hit hit, float maxDistance = 100f)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		hit = default(Hit);
		ref Int3 pos = ref hit.pos;
		Vector3 origin = ray.get_origin();
		pos.x = Mathf.FloorToInt(origin.x);
		ref Int3 pos2 = ref hit.pos;
		Vector3 origin2 = ray.get_origin();
		pos2.y = Mathf.FloorToInt(origin2.y);
		ref Int3 pos3 = ref hit.pos;
		Vector3 origin3 = ray.get_origin();
		pos3.z = Mathf.FloorToInt(origin3.z);
		hit.prevPos = hit.pos;
		Vector3 direction = ray.get_direction();
		int num;
		if (direction.x > 0f)
		{
			num = 1;
		}
		else
		{
			Vector3 direction2 = ray.get_direction();
			num = ((direction2.x < 0f) ? (-1) : 0);
		}
		int num2 = num;
		Vector3 direction3 = ray.get_direction();
		int num3;
		if (direction3.y > 0f)
		{
			num3 = 1;
		}
		else
		{
			Vector3 direction4 = ray.get_direction();
			num3 = ((direction4.y < 0f) ? (-1) : 0);
		}
		int num4 = num3;
		Vector3 direction5 = ray.get_direction();
		int num5;
		if (direction5.z > 0f)
		{
			num5 = 1;
		}
		else
		{
			Vector3 direction6 = ray.get_direction();
			num5 = ((direction6.z < 0f) ? (-1) : 0);
		}
		int num6 = num5;
		float num7;
		if (num2 != 0)
		{
			Vector3 direction7 = ray.get_direction();
			num7 = 1f / Mathf.Abs(direction7.x);
		}
		else
		{
			num7 = 99999f;
		}
		float num8 = num7;
		float num9;
		if (num4 != 0)
		{
			Vector3 direction8 = ray.get_direction();
			num9 = 1f / Mathf.Abs(direction8.y);
		}
		else
		{
			num9 = 99999f;
		}
		float num10 = num9;
		float num11;
		if (num6 != 0)
		{
			Vector3 direction9 = ray.get_direction();
			num11 = 1f / Mathf.Abs(direction9.z);
		}
		else
		{
			num11 = 99999f;
		}
		float num12 = num11;
		float num13;
		switch (num2)
		{
		case 1:
		{
			Vector3 origin6 = ray.get_origin();
			float num14 = Mathf.Ceil(origin6.x);
			Vector3 origin7 = ray.get_origin();
			num13 = (num14 - origin7.x) * num8;
			break;
		}
		default:
		{
			Vector3 origin4 = ray.get_origin();
			float x = origin4.x;
			Vector3 origin5 = ray.get_origin();
			num13 = (x - Mathf.Floor(origin5.x)) * num8;
			break;
		}
		case 0:
			num13 = 99999f;
			break;
		}
		float num15;
		switch (num4)
		{
		case 1:
		{
			Vector3 origin10 = ray.get_origin();
			float num16 = Mathf.Ceil(origin10.y);
			Vector3 origin11 = ray.get_origin();
			num15 = (num16 - origin11.y) * num10;
			break;
		}
		default:
		{
			Vector3 origin8 = ray.get_origin();
			float y = origin8.y;
			Vector3 origin9 = ray.get_origin();
			num15 = (y - Mathf.Floor(origin9.y)) * num10;
			break;
		}
		case 0:
			num15 = 99999f;
			break;
		}
		float num17;
		switch (num6)
		{
		case 1:
		{
			Vector3 origin14 = ray.get_origin();
			float num18 = Mathf.Ceil(origin14.z);
			Vector3 origin15 = ray.get_origin();
			num17 = (num18 - origin15.z) * num12;
			break;
		}
		default:
		{
			Vector3 origin12 = ray.get_origin();
			float z = origin12.z;
			Vector3 origin13 = ray.get_origin();
			num17 = (z - Mathf.Floor(origin13.z)) * num12;
			break;
		}
		case 0:
			num17 = 99999f;
			break;
		}
		do
		{
			hit.prevPos = hit.pos;
			if (num13 < num15)
			{
				if (num13 < num17)
				{
					hit.pos.x += num2;
					if (num13 > maxDistance)
					{
						return false;
					}
					num13 += num8;
				}
				else
				{
					hit.pos.z += num6;
					if (num17 > maxDistance)
					{
						return result;
					}
					num17 += num12;
				}
			}
			else if (num15 < num17)
			{
				hit.pos.y += num4;
				if (num15 > maxDistance)
				{
					return result;
				}
				num15 += num10;
			}
			else
			{
				hit.pos.z += num6;
				if (num17 > maxDistance)
				{
					return result;
				}
				num17 += num12;
			}
		}
		while (!hitFunc(hit.pos));
		return true;
	}
}
