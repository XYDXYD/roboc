using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedVector3Int : SharedVariable<Vector3Int>
	{
		public static implicit operator SharedVector3Int(Vector3Int value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SharedVector3Int sharedVector3Int = new SharedVector3Int();
			sharedVector3Int.mValue = (_00210)value;
			return sharedVector3Int;
		}
	}
}
