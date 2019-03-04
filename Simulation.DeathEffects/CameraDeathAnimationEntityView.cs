using Svelto.ECS;

namespace Simulation.DeathEffects
{
	internal class CameraDeathAnimationEntityView : EntityView
	{
		public IDeathAnimationComponent deathAnimationComponent;

		public IDeathAnimationBroadcastComponent deathAnimationBroadcastComponent;

		public CameraDeathAnimationEntityView()
			: this()
		{
		}
	}
}
