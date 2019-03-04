using Svelto.ECS;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal class RocketLauncherProjectileTrailNode : EntityView
	{
		public IRocketLauncherProjectileTrailEffects trailEffects;

		public IProjectileAliveComponent projectileAliveComponent;

		public IProjectileTimeComponent projectileTimeComponent;

		public ITransformComponent transformComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public IWeaponIdComponent weaponIdComponent;

		public IProjectileMovementStatsComponent movementComponent;

		public RocketLauncherProjectileTrailNode()
			: this()
		{
		}
	}
}
