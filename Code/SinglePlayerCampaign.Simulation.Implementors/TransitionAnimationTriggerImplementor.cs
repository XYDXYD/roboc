using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Implementors
{
	internal class TransitionAnimationTriggerImplementor : ITransitionAnimationTriggerComponent
	{
		public DispatchOnSet<bool> WaveCompleted
		{
			get;
			private set;
		}

		public TransitionAnimationTriggerImplementor(int entityID)
		{
			WaveCompleted = new DispatchOnSet<bool>(entityID);
		}
	}
}
