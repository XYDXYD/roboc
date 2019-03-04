using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadRadarModuleStatsNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IRadarStatsComponent radarStatsComponent;

		public LoadRadarModuleStatsNode()
			: this()
		{
		}
	}
}
