using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Nano
{
	internal sealed class HealingProjectileImpactNode : EntityView
	{
		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponDamageComponent weaponDamageComponent;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IProjectileOwnerComponent projectileOwnerComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IEntitySourceComponent entitySourceComponent;

		public HealingProjectileImpactNode()
			: this()
		{
		}
	}
}
