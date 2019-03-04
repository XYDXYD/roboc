using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedAnimationCurve : SharedVariable<AnimationCurve>
	{
		public static implicit operator SharedAnimationCurve(AnimationCurve value)
		{
			SharedAnimationCurve sharedAnimationCurve = new SharedAnimationCurve();
			sharedAnimationCurve.mValue = (_00210)value;
			return sharedAnimationCurve;
		}
	}
}
