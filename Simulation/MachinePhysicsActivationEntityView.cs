using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal sealed class MachinePhysicsActivationEntityView : EntityView
	{
		public IRigidBodyComponent rigidbodyComponent;

		public ISpawnableComponent spawnableComponent;

		public MachinePhysicsActivationEntityView()
			: this()
		{
		}
	}
}
