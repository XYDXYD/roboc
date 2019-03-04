using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignVictoryCheckEntityView : EntityView
	{
		public ICounterComponent currentWaveComponent;

		public ICampaignVictoryComponent campaignVictoryComponent;

		public CampaignVictoryCheckEntityView()
			: this()
		{
		}
	}
}
