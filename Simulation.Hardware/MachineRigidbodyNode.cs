using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineRigidbodyNode : EntityView
	{
		public IRigidBodyComponent rigidbodyComponent;

		public IMachineOwnerComponent ownerComponent;

		public MachineRigidbodyNode()
			: this()
		{
		}
	}
}
