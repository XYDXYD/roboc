using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignUpdateRemainingLivesEntityView : EntityView
	{
		public IRemainingLivesComponent remainingLivesComponent;

		public CampaignUpdateRemainingLivesEntityView()
			: this()
		{
		}
	}
}
