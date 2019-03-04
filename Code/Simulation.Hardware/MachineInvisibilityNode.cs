using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineInvisibilityNode : EntityView
	{
		public IMachineVisibilityComponent machineVisibilityComponent;

		public IMachineOwnerComponent ownerComponent;

		public IManaDrainComponent manaDrainComponent;

		public ICloakStatsComponent cloakStatsComponent;

		public MachineInvisibilityNode()
			: this()
		{
		}
	}
}
