using Svelto.ECS;

namespace Simulation.Hardware.Movement
{
	internal sealed class MovementStatsNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IMaxSpeedStatsComponent statsComponent;

		public IMaxCarryMassComponent maxCarryMassComponent;

		public MovementStatsNode()
			: this()
		{
		}
	}
}
