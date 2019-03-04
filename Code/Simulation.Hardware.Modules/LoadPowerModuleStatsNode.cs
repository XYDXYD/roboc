using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadPowerModuleStatsNode : EntityView
	{
		public IPowerModuleStatsComponent statsComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public LoadPowerModuleStatsNode()
			: this()
		{
		}
	}
}
