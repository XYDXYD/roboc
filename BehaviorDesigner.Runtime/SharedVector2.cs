using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedVector2 : SharedVariable<Vector2>
	{
		public static implicit operator SharedVector2(Vector2 value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SharedVector2 sharedVector = new SharedVector2();
			sharedVector.mValue = (_00210)value;
			return sharedVector;
		}
	}
}
