using System;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedBool : SharedVariable<bool>
	{
		public static implicit operator SharedBool(bool value)
		{
			//IL_000d: Expected O, but got I4
			SharedBool sharedBool = new SharedBool();
			sharedBool.mValue = (_00210)value;
			return sharedBool;
		}
	}
}
