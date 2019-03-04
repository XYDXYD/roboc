using System;

namespace BehaviorDesigner.Runtime
{
	[Serializable]
	public class SharedInt : SharedVariable<int>
	{
		public static implicit operator SharedInt(int value)
		{
			//IL_000d: Expected O, but got I4
			SharedInt sharedInt = new SharedInt();
			sharedInt.mValue = (_00210)value;
			return sharedInt;
		}
	}
}
