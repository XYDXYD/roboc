using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class HomingProjectileNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IProjectileMovementStatsComponent projectileMovementStats;

		public IProjectileOwnerComponent projectileOwnerComponent;

		public IProjectileTimeComponent projectileTimeComponent;

		public IProjectileRangeComponent projectileRangeComponent;

		public IProjectileAliveComponent projectileAliveComponent;

		public IHomingProjectileStatsComponent rocketLauncherProjectileStats;

		public IHitSomethingComponent hitSomethingComponent;

		public ILockOnTargetComponent lockOnComponent;

		public IWeaponDamageComponent weaponDamageComponent;

		public IEntitySourceComponent entitySourceComponent;

		public HomingProjectileNode()
			: this()
		{
		}
	}
}
