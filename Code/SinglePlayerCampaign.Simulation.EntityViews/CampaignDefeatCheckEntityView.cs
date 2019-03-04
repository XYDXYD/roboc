using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignDefeatCheckEntityView : EntityView
	{
		public IRemainingLivesComponent remainingLivesComponent;

		public ICampaignDefeatComponent campaignDefeatComponent;

		public CampaignDefeatCheckEntityView()
			: this()
		{
		}
	}
}
