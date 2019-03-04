using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer
{
	[Serializable]
	public class SharedPathNodeArray : SharedVariable<PathNode[]>
	{
		public static implicit operator SharedPathNodeArray(PathNode[] value)
		{
			SharedPathNodeArray sharedPathNodeArray = new SharedPathNodeArray();
			sharedPathNodeArray.set_Value(value);
			return sharedPathNodeArray;
		}
	}
}
