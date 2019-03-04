using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveLostLivesEntityView : EntityView
	{
		public ICounterComponent currentWaveLostLivesComponent;

		public CampaignWaveLostLivesEntityView()
			: this()
		{
		}
	}
}
