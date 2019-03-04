using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedRect : SharedVariable<Rect>
	{
		public static implicit operator SharedRect(Rect value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SharedRect sharedRect = new SharedRect();
			sharedRect.mValue = (_00210)value;
			return sharedRect;
		}
	}
}
