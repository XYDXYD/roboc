using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveDefeatCheckEntityView : EntityView
	{
		public ITimeComponent timeComponent;

		public IWaveDefeatComponent waveDefeatComponent;

		public CampaignWaveDefeatCheckEntityView()
			: this()
		{
		}
	}
}
