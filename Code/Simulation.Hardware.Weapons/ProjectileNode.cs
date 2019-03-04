using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class ProjectileNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public IWeaponDamageComponent weaponDamageComponent;

		public IWeaponIdComponent weaponIdComponent;

		public IProjectileMovementStatsComponent projectileMovementStats;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IProjectileOwnerComponent projectileOwnerComponent;

		public IProjectileTimeComponent projectileTimeComponent;

		public IProjectileRangeComponent projectileRangeComponent;

		public IProjectileAliveComponent projectileAliveComponent;

		public IProjectileStateComponent projectileStateComponent;

		public IPredictedProjectilePositionComponent predictedProjectilePosition;

		public IHitSomethingComponent hitSomethingComponent;

		public IEntitySourceComponent entitySourceComponent;

		public ProjectileNode()
			: this()
		{
		}
	}
}
