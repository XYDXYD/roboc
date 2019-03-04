using Svelto.ECS;

namespace Simulation.Hardware.Movement
{
	internal sealed class StrafingCustomAngleToStraightNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IStrafingCustomAngleToStraightComponent customAngleToStraightComponent;

		public IStrafingCustomInputComponent customInputComponent;

		public StrafingCustomAngleToStraightNode()
			: this()
		{
		}
	}
}
