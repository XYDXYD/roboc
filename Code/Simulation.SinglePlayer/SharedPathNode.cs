using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer
{
	[Serializable]
	public class SharedPathNode : SharedVariable<PathNode>
	{
		public static implicit operator SharedPathNode(PathNode value)
		{
			SharedPathNode sharedPathNode = new SharedPathNode();
			sharedPathNode.set_Value(value);
			return sharedPathNode;
		}
	}
}
