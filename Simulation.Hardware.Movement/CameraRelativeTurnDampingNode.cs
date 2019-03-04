using Svelto.ECS;

namespace Simulation.Hardware.Movement
{
	internal sealed class CameraRelativeTurnDampingNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidBodyComponent;

		public IHardwareDisabledComponent disabledComponent;

		public CameraRelativeTurnDampingNode()
			: this()
		{
		}
	}
}
