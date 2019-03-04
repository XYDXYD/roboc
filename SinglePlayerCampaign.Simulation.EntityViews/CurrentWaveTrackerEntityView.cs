using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CurrentWaveTrackerEntityView : EntityView
	{
		public ICounterComponent CurrentWaveCounterComponent;

		public IReadyToSpawnWaveComponent ReadyToSpawnWaveComponent;

		public ITransitionAnimationTriggerComponent TransitionAnimationTriggerComponent;

		public CurrentWaveTrackerEntityView()
			: this()
		{
		}
	}
}
