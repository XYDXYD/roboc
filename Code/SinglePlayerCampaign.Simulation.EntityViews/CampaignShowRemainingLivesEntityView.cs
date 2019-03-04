using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignShowRemainingLivesEntityView : EntityView
	{
		public IRemainingLivesComponent remainingLivesComponent;

		public CampaignShowRemainingLivesEntityView()
			: this()
		{
		}
	}
}
