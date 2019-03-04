using Svelto.ECS;

namespace Simulation.Hardware.Weapons.AeroFlak
{
	internal sealed class LoadAeroflakWeaponStatsNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IAeroflakProjectileStatsComponent aeroflakProjectileStats;

		public IStackDamageStatsComponent stackDamageStats;

		public LoadAeroflakWeaponStatsNode()
			: this()
		{
		}
	}
}
