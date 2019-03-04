using Simulation.Hardware.Weapons;
using Svelto.ECS;

namespace Simulation.Hardware.Modules
{
	internal sealed class LoadModuleStatsNode : EntityView
	{
		public IWeaponFireCostComponent manaComponent;

		public IWeaponCooldownComponent cooldownComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IModuleRangeComponent rangeComponent;

		public LoadModuleStatsNode()
			: this()
		{
		}
	}
}
