using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer.BehaviorTree
{
	[Serializable]
	public class SharedPathFollowingState : SharedVariable<PathFollowingState>
	{
		public static implicit operator SharedPathFollowingState(PathFollowingState value)
		{
			SharedPathFollowingState sharedPathFollowingState = new SharedPathFollowingState();
			sharedPathFollowingState.set_Value(value);
			return sharedPathFollowingState;
		}
	}
}
