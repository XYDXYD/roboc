using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveVictoryCheckEntityView : EntityView
	{
		public ITimeComponent timeComponent;

		public IKillCountComponent killCountComponent;

		public IWaveVictoryComponent waveVictoryComponent;

		public CampaignWaveVictoryCheckEntityView()
			: this()
		{
		}
	}
}
