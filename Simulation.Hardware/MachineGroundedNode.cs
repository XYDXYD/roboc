using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineGroundedNode : EntityView
	{
		public IRigidBodyComponent rigidbodyComponent;

		public IMachineGroundedComponent machineGroundedComponent;

		public IMachineOwnerComponent ownerComponent;

		public MachineGroundedNode()
			: this()
		{
		}
	}
}
