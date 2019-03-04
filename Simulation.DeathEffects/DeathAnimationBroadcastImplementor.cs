using Svelto.ECS;

namespace Simulation.DeathEffects
{
	internal class DeathAnimationBroadcastImplementor : IDeathAnimationBroadcastComponent
	{
		public DispatchOnChange<bool> isAnimating
		{
			get;
			private set;
		}

		public DeathAnimationBroadcastImplementor(int entityId)
		{
			isAnimating = new DispatchOnChange<bool>(entityId);
		}
	}
}
