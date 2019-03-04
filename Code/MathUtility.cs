using System;
using UnityEngine;

internal sealed class MathUtility
{
	private static readonly float INV_ROOT_2_PI = 1f / Mathf.Sqrt((float)Math.PI * 2f);

	public static float RandomGausianDistribution()
	{
		float num = Random.Range(0f, 2f);
		float num2 = Mathf.Exp((0f - num) * num * 0.5f) * INV_ROOT_2_PI;
		return 1f - num2 * 2.5f;
	}

	public static float RandomRotation()
	{
		return Random.Range(0f, 360f);
	}

	public static Vector3 ClosetPointOnLine(Vector3 A, Vector3 B, Vector3 P)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (A == B)
		{
			return A;
		}
		Vector3 val = P - A;
		Vector3 val2 = B - A;
		float num = val2.x * val2.x + val2.y * val2.y;
		float num2 = val.x * val2.x + val.y * val2.y;
		float num3 = num2 / num;
		if (num3 < 0f)
		{
			num3 = 0f;
		}
		else if (num3 > 1f)
		{
			num3 = 1f;
		}
		return A + val2 * num3;
	}
}
