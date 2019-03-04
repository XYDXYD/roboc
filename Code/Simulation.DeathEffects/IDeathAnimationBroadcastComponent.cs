using Svelto.ECS;

namespace Simulation.DeathEffects
{
	internal interface IDeathAnimationBroadcastComponent
	{
		DispatchOnChange<bool> isAnimating
		{
			get;
		}
	}
}
