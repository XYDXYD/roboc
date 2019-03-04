using BehaviorDesigner.Runtime;
using Simulation.SinglePlayer;
using Svelto.DataStructures;
using System;

[Serializable]
internal class SharedAIGameObjectMovementDataList : SharedVariable<FasterList<AIAgentDataComponentsNode>>
{
	public static implicit operator SharedAIGameObjectMovementDataList(FasterList<AIAgentDataComponentsNode> value)
	{
		SharedAIGameObjectMovementDataList sharedAIGameObjectMovementDataList = new SharedAIGameObjectMovementDataList();
		sharedAIGameObjectMovementDataList.set_Value(value);
		return sharedAIGameObjectMovementDataList;
	}
}
