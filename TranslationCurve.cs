using System;
using UnityEngine;

[Serializable]
public class TranslationCurve : MovementCurve
{
	public AnimationCurve[] x = (AnimationCurve[])new AnimationCurve[1]
	{
		new AnimationCurve((Keyframe[])new Keyframe[2]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 0f)
		})
	};

	public AnimationCurve[] y = (AnimationCurve[])new AnimationCurve[1]
	{
		new AnimationCurve((Keyframe[])new Keyframe[2]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 0f)
		})
	};

	public AnimationCurve[] z = (AnimationCurve[])new AnimationCurve[1]
	{
		new AnimationCurve((Keyframe[])new Keyframe[2]
		{
			new Keyframe(0f, 0f),
			new Keyframe(1f, 0f)
		})
	};

	public override void ValidateInInspector()
	{
		ValidateSingleAxis(ref x);
		ValidateSingleAxis(ref y);
		ValidateSingleAxis(ref z);
	}
}
