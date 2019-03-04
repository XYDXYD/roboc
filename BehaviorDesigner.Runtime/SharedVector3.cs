using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedVector3 : SharedVariable<Vector3>
	{
		public static implicit operator SharedVector3(Vector3 value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SharedVector3 sharedVector = new SharedVector3();
			sharedVector.mValue = (_00210)value;
			return sharedVector;
		}
	}
}
