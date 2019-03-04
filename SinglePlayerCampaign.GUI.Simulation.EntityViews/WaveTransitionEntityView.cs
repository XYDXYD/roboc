using SinglePlayerCampaign.GUI.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.GUI.Simulation.EntityViews
{
	public class WaveTransitionEntityView : EntityView
	{
		public ITransitionAnimationComponent AnimationComponent;

		public WaveTransitionEntityView()
			: this()
		{
		}
	}
}
