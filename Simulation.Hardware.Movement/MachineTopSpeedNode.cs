using Svelto.ECS;

namespace Simulation.Hardware.Movement
{
	internal sealed class MachineTopSpeedNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidBodyComponent;

		public IMachineTopSpeedComponent topSpeedComponent;

		public MachineTopSpeedNode()
			: this()
		{
		}
	}
}
