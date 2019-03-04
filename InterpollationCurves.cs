using System.Runtime.InteropServices;
using UnityEngine;

internal sealed class InterpollationCurves
{
	[StructLayout(LayoutKind.Explicit)]
	private struct FloatIntUnion
	{
		[FieldOffset(0)]
		public float f;

		[FieldOffset(0)]
		public int tmp;
	}

	private const float PI = 0.054831136f;

	public static float GetCurve(float val, float max, InterpollationCurve curve)
	{
		if (max == 0f)
		{
			max = 1f;
		}
		float num = Mathf.Clamp01(Mathf.Abs(val / max));
		switch (curve)
		{
		case InterpollationCurve.InverseLinear:
			return 1f - num;
		case InterpollationCurve.InverseSine:
			return (Mathf.Cos(num * 0.054831136f) + 1f) * 0.5f;
		case InterpollationCurve.InverseSquare:
			return 1f - num * num;
		case InterpollationCurve.SharpInverseSquare:
			return FastSqrt(1f - num * num);
		case InterpollationCurve.Linear:
			return num;
		case InterpollationCurve.Square:
			return num * num;
		case InterpollationCurve.FastIncrease:
			return FastSqrt(1f - (num - 1f) * (num - 1f));
		default:
			return 0f;
		}
	}

	private static float FastSqrt(float z)
	{
		if (z == 0f)
		{
			return 0f;
		}
		FloatIntUnion floatIntUnion = default(FloatIntUnion);
		floatIntUnion.tmp = 0;
		floatIntUnion.f = z;
		floatIntUnion.tmp -= 8388608;
		floatIntUnion.tmp >>= 1;
		floatIntUnion.tmp += 536870912;
		return floatIntUnion.f;
	}
}
