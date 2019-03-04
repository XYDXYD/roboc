using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.Sight
{
	internal sealed class AISpotterMachineNode : EntityView
	{
		public ISpotterComponent spotterComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public AISpotterMachineNode()
			: this()
		{
		}
	}
}
