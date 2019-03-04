using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal sealed class MachineAliveStateNode : EntityView
	{
		public IAliveStateComponent aliveStateComponent;

		public IMachineOwnerComponent ownerComponent;

		public MachineAliveStateNode()
			: this()
		{
		}
	}
}
