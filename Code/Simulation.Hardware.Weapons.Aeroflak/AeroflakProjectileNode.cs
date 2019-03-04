using Svelto.ECS;

namespace Simulation.Hardware.Weapons.AeroFlak
{
	internal sealed class AeroflakProjectileNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IProjectileMovementStatsComponent projectileMovementStats;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IProjectileOwnerComponent projectileOwnerComponent;

		public IProjectileTimeComponent projectileTimeComponent;

		public IProjectileRangeComponent projectileRangeComponent;

		public IProjectileAliveComponent projectileAliveComponent;

		public IAeroflakProjectileStatsComponent aeroflakProjectileStats;

		public IWeaponIdComponent weaponIdComponent;

		public ISplashDamageComponent splashComponent;

		public IStackDamageComponent stackDamageComponent;

		public IEntitySourceComponent entitySourceComponent;

		public AeroflakProjectileNode()
			: this()
		{
		}
	}
}
