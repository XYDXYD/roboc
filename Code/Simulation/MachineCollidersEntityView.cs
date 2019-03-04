using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal class MachineCollidersEntityView : EntityView
	{
		public IMachineCollidersComponent machineCollidersComponent;

		public IMachineOwnerComponent machineOwnerComponent;

		public MachineCollidersEntityView()
			: this()
		{
		}
	}
}
