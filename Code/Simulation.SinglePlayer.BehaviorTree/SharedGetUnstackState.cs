using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer.BehaviorTree
{
	[Serializable]
	public class SharedGetUnstackState : SharedVariable<GetUnstuckState>
	{
		public static implicit operator SharedGetUnstackState(GetUnstuckState value)
		{
			SharedGetUnstackState sharedGetUnstackState = new SharedGetUnstackState();
			sharedGetUnstackState.set_Value(value);
			return sharedGetUnstackState;
		}
	}
}
