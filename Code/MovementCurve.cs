using System;
using UnityEngine;
using Utility;

[Serializable]
public abstract class MovementCurve
{
	public abstract void ValidateInInspector();

	public static float? Evaluate(AnimationCurve[] curves, float time, float duration, Random random)
	{
		if (curves.Length > 0)
		{
			if (curves.Length > 1)
			{
				if (CheckTimeIsValid(curves[0], time, duration) && CheckTimeIsValid(curves[1], time, duration))
				{
					if (EvaluateSingleCurve(curves[0], time, duration) < EvaluateSingleCurve(curves[1], time, duration))
					{
						return RandomNumberBetween(random, EvaluateSingleCurve(curves[0], time, duration), EvaluateSingleCurve(curves[1], time, duration));
					}
					return RandomNumberBetween(random, EvaluateSingleCurve(curves[1], time, duration), EvaluateSingleCurve(curves[0], time, duration));
				}
				if (CheckTimeIsValid(curves[1], time, duration))
				{
					return EvaluateSingleCurve(curves[1], time, duration);
				}
			}
			if (CheckTimeIsValid(curves[0], time, duration))
			{
				return EvaluateSingleCurve(curves[0], time, duration);
			}
		}
		return null;
	}

	private static bool CheckTimeIsValid(AnimationCurve curve, float time, float duration)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Keyframe val = curve.get_Item(curve.get_length() - 1);
		float time2 = val.get_time();
		float num = time * time2 / duration;
		return num <= time2;
	}

	private static float EvaluateSingleCurve(AnimationCurve curve, float time, float duration)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Keyframe val = curve.get_Item(curve.get_length() - 1);
		float time2 = val.get_time();
		float num = time * time2 / duration;
		return curve.Evaluate(num);
	}

	protected void ValidateSingleAxis(ref AnimationCurve[] curves)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		if (curves == null)
		{
			curves = (AnimationCurve[])new AnimationCurve[1];
		}
		else if (curves.Length > 2)
		{
			Console.LogWarning("We only handle a random value between 2 curves.");
			Array.Resize(ref curves, 2);
		}
		else if (curves.Length == 0)
		{
			Console.LogWarning("There need to be at least 1 curve.");
			Array.Resize(ref curves, 1);
			curves[0] = new AnimationCurve((Keyframe[])new Keyframe[2]
			{
				new Keyframe(0f, 0f),
				new Keyframe(1f, 0f)
			});
		}
	}

	private static float RandomNumberBetween(Random random, double minValue, double maxValue)
	{
		double num = random.NextDouble();
		return (float)(minValue + num * (maxValue - minValue));
	}
}
