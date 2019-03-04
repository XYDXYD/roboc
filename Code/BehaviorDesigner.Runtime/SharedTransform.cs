using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedTransform : SharedVariable<Transform>
	{
		public static implicit operator SharedTransform(Transform value)
		{
			SharedTransform sharedTransform = new SharedTransform();
			sharedTransform.mValue = (_00210)value;
			return sharedTransform;
		}
	}
}
