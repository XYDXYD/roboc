using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class SplashProjectileNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IProjectileOwnerComponent ownerComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IProjectileMovementStatsComponent projectileMovementStats;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IProjectileTimeComponent projectileTimeComponent;

		public IProjectileRangeComponent projectileRangeComponent;

		public IProjectileAliveComponent projectileAliveComponent;

		public ISplashDamageComponent splashComponent;

		public IWeaponIdComponent weaponIdComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IWeaponOwnerPositionComponent ownerPositionComponent;

		public IEntitySourceComponent entitySourceComponent;

		public SplashProjectileNode()
			: this()
		{
		}
	}
}
