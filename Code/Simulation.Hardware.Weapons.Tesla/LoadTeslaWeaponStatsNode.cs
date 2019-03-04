using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal sealed class LoadTeslaWeaponStatsNode : EntityView
	{
		public IProjectileDamageStatsComponent projectileDamageStats;

		public ITeslaDamageStats teslaDamageStats;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponCategoryComponent itemCategory;

		public IWeaponFireCostComponent weaponFireCostComponent;

		public IFireTimingComponent fireTimingStats;

		public LoadTeslaWeaponStatsNode()
			: this()
		{
		}
	}
}
