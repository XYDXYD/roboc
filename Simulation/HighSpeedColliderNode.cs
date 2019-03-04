using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal sealed class HighSpeedColliderNode : EntityView
	{
		public IRigidBodyComponent rigidbodyComponent;

		public IMachineOwnerComponent ownerComponent;

		public HighSpeedColliderNode()
			: this()
		{
		}
	}
}
