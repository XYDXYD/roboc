using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class HealingPriorityNode : EntityView
	{
		public IHealingPriorityComponent healingPriorityComponent;

		public HealingPriorityNode()
			: this()
		{
		}
	}
}
