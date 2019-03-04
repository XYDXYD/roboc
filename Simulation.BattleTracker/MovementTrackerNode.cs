using Simulation.Hardware;
using Svelto.ECS;

namespace Simulation.BattleTracker
{
	internal sealed class MovementTrackerNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public MovementTrackerNode()
			: this()
		{
		}
	}
}
