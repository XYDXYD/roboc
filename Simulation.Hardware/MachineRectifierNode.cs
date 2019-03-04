using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineRectifierNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IMachineFunctionalComponent machineFunctionalsComponent;

		public MachineRectifierNode()
			: this()
		{
		}
	}
}
