using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Plasma
{
	internal sealed class LoadPlasmaWeaponStatsNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IPlasmaProjectileStatsComponent plasmaProjectileStats;

		public LoadPlasmaWeaponStatsNode()
			: this()
		{
		}
	}
}
