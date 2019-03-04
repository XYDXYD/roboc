using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveShowGoalsEntityView : EntityView
	{
		public IKillCountComponent killCountComponent;

		public ITimeComponent timeComponent;

		public IWaveVictoryComponent waveVictoryComponent;

		public IWaveDefeatComponent waveDefeatComponent;

		public CampaignWaveShowGoalsEntityView()
			: this()
		{
		}
	}
}
