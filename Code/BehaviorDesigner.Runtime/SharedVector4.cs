using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedVector4 : SharedVariable<Vector4>
	{
		public static implicit operator SharedVector4(Vector4 value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SharedVector4 sharedVector = new SharedVector4();
			sharedVector.mValue = (_00210)value;
			return sharedVector;
		}
	}
}
