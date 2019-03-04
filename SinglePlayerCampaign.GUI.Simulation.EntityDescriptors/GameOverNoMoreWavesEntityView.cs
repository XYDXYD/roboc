using SinglePlayerCampaign.GUI.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.GUI.Simulation.EntityDescriptors
{
	public class GameOverNoMoreWavesEntityView : EntityView
	{
		public ITransitionAnimationComponent AnimationComponent;

		public GameOverNoMoreWavesEntityView()
			: this()
		{
		}
	}
}
