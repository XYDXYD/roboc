using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal class RocketLauncherImpactNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IProjectileDamageStatsComponent projectileDamageStats;

		public IProjectileOwnerComponent projectileOwnerComponent;

		public IHomingProjectileStatsComponent rocketLauncherProjectileStats;

		public IHitSomethingComponent hitSomethingComponent;

		public IItemDescriptorComponent itemDescriptorComponent;

		public ISplashDamageComponent splashComponent;

		public IWeaponDamageComponent damageComponent;

		public IEntitySourceComponent entitySourceComponent;

		public RocketLauncherImpactNode()
			: this()
		{
		}
	}
}
