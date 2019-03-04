using Svelto.ECS;

namespace Simulation.GUI
{
	internal class HealthBarShowEntityView : EntityView
	{
		public IHealthBarComponent healthBarComponent;

		public IHealthBarMachineIdComponent healthBarMachineIdComponent;

		public HealthBarShowEntityView()
			: this()
		{
		}
	}
}
