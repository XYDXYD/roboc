using Svelto.ECS;

namespace Simulation.Hardware.Movement
{
	internal sealed class CameraRelativeInputNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IMachineInputComponent machineControlComponent;

		public IStrafingCustomAngleToStraightComponent strafingCustomAngleToStraightComponent;

		public IStrafingCustomInputComponent strafingCustomInputComponent;

		public CameraRelativeInputNode()
			: this()
		{
		}
	}
}
