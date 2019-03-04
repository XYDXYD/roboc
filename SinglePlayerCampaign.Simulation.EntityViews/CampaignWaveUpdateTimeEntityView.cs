using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveUpdateTimeEntityView : EntityView
	{
		public ITimeComponent timeComponent;

		public CampaignWaveUpdateTimeEntityView()
			: this()
		{
		}
	}
}
