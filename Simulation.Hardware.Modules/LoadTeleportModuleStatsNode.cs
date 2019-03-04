using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadTeleportModuleStatsNode : EntityView
	{
		public ITeleportModuleSettingsComponent settingsComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public LoadTeleportModuleStatsNode()
			: this()
		{
		}
	}
}
