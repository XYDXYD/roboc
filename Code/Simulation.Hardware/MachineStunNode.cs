using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineStunNode : EntityView
	{
		public IMachineStunComponent stunComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineOwnerComponent ownerComponent;

		public MachineStunNode()
			: this()
		{
		}
	}
}
