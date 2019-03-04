using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class LoadWeaponStatsNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponCategoryComponent itemCategory;

		public IWeaponFireCostComponent weaponFireCostComponent;

		public IFireTimingComponent fireTimingStats;

		public IMisfireComponent weaponMisfireComponent;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IWeaponAccuracyStatsComponent accuracyStats;

		public IProjectileRangeComponent projectileRangeStats;

		public IProjectileSpeedStatsComponent projectileSpeedStats;

		public LoadWeaponStatsNode()
			: this()
		{
		}
	}
}
