using SinglePlayerCampaign.GUI.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.GUI.Simulation.EntityDescriptors
{
	public class GameOverPlayerEntityView : EntityView
	{
		public ITransitionAnimationComponent AnimationComponent;

		public GameOverPlayerEntityView()
			: this()
		{
		}
	}
}
