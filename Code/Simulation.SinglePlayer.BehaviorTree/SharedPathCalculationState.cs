using BehaviorDesigner.Runtime;
using System;

namespace Simulation.SinglePlayer.BehaviorTree
{
	[Serializable]
	public class SharedPathCalculationState : SharedVariable<PathCalculationState>
	{
		public static implicit operator SharedPathCalculationState(PathCalculationState value)
		{
			SharedPathCalculationState sharedPathCalculationState = new SharedPathCalculationState();
			sharedPathCalculationState.set_Value(value);
			return sharedPathCalculationState;
		}
	}
}
