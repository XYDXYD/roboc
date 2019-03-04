using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedVector2Int : SharedVariable<Vector2Int>
	{
		public static implicit operator SharedVector2Int(Vector2Int value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SharedVector2Int sharedVector2Int = new SharedVector2Int();
			sharedVector2Int.mValue = (_00210)value;
			return sharedVector2Int;
		}
	}
}
