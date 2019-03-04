using Simulation.Common;
using Svelto.ECS;

namespace Simulation.Hardware.Weapons.Plasma
{
	internal sealed class PlasmaProjectileNode : EntityView
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

		public IGravityComponent gravityComponent;

		public IPlasmaProjectileStatsComponent plasmaProjectileComponent;

		public IWeaponOwnerPositionComponent ownerPositionComponent;

		public IEntitySourceComponent entitySourceComponent;

		public PlasmaProjectileNode()
			: this()
		{
		}
	}
}
