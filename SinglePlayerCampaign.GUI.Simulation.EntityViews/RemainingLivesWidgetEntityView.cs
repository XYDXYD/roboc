using SinglePlayerCampaign.GUI.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.GUI.Simulation.EntityViews
{
	internal class RemainingLivesWidgetEntityView : EntityView
	{
		public IWidgetCounterComponent RemainingPlayerLivesWidgetCounterComponent;

		public RemainingLivesWidgetEntityView()
			: this()
		{
		}
	}
}
