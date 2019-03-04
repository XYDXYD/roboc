using Svelto.ECS;

namespace Simulation
{
	internal class PowerBarConsumptionEntityView : EntityView
	{
		public IDeltaTimeComponent deltaTimeComponent;

		public PowerBarConsumptionEntityView()
			: this()
		{
		}
	}
}
