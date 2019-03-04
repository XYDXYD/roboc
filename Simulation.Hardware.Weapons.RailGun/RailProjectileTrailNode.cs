using Svelto.ECS;

namespace Simulation.Hardware.Weapons.RailGun
{
	internal sealed class RailProjectileTrailNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IRailProjectileTrailComponent trailEffects;

		public IProjectileMovementStatsComponent movementComponent;

		public IProjectileAliveComponent projectileAliveComponent;

		public IProjectileTimeComponent projectileTimeComponent;

		public IHitSomethingComponent hitSomethingComponent;

		public RailProjectileTrailNode()
			: this()
		{
		}
	}
}
