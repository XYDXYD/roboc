using Svelto.ECS;

namespace Simulation.Hardware.Weapons
{
	internal sealed class GenericProjectileTrailNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IProjectileAliveComponent projectileAliveComponent;

		public IProjectileTrailEffectsComponent TrailEffectsComponent;

		public IProjectileMovementStatsComponent movementComponent;

		public IProjectileAliveComponent projectileAliveCOmponent;

		public IProjectileTimeComponent projectileTimeComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public GenericProjectileTrailNode()
			: this()
		{
		}
	}
}
