using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedColor : SharedVariable<Color>
	{
		public static implicit operator SharedColor(Color value)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Expected O, but got Unknown
			SharedColor sharedColor = new SharedColor();
			sharedColor.mValue = (_00210)value;
			return sharedColor;
		}
	}
}
