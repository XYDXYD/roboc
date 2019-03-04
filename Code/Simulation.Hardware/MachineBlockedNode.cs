using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineBlockedNode : EntityView
	{
		public IMachineOwnerComponent machineOwnerComponent;

		public MachineBlockedNode()
			: this()
		{
		}
	}
}
