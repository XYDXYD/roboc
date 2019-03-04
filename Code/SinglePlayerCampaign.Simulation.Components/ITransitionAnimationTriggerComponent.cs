using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Components
{
	public interface ITransitionAnimationTriggerComponent
	{
		DispatchOnSet<bool> WaveCompleted
		{
			get;
		}
	}
}
