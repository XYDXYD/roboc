using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer
{
	[Serializable]
	public class SharedAIBehaviorTreeAgentNode : SharedVariable<AIAgentDataComponentsNode>
	{
		public static implicit operator SharedAIBehaviorTreeAgentNode(AIAgentDataComponentsNode value)
		{
			SharedAIBehaviorTreeAgentNode sharedAIBehaviorTreeAgentNode = new SharedAIBehaviorTreeAgentNode();
			sharedAIBehaviorTreeAgentNode.set_Value(value);
			return sharedAIBehaviorTreeAgentNode;
		}
	}
}
