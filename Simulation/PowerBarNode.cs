using Svelto.ECS;

namespace Simulation
{
	internal sealed class PowerBarNode : EntityView
	{
		public IPowerBarDataComponent powerBarDataComponent;

		public PowerBarNode()
			: this()
		{
		}
	}
}
