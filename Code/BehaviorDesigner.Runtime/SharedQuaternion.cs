using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedQuaternion : SharedVariable<Quaternion>
	{
		public static implicit operator SharedQuaternion(Quaternion value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SharedQuaternion sharedQuaternion = new SharedQuaternion();
			sharedQuaternion.mValue = (_00210)value;
			return sharedQuaternion;
		}
	}
}
