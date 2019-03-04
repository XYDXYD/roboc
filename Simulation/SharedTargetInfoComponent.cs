using BehaviorDesigner.Runtime;
using System;

namespace Simulation
{
	[Serializable]
	public class SharedTargetInfoComponent : SharedVariable<TargetInfo>
	{
		public static implicit operator SharedTargetInfoComponent(TargetInfo value)
		{
			SharedTargetInfoComponent sharedTargetInfoComponent = new SharedTargetInfoComponent();
			sharedTargetInfoComponent.set_Value(value);
			return sharedTargetInfoComponent;
		}
	}
}
