using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation
{
	internal class AutoHealEntityView : EntityView
	{
		public IAutoHealComponent autoHealComponent;

		public IMachineOwnerComponent ownerComponent;

		public AutoHealEntityView()
			: this()
		{
		}
	}
}
