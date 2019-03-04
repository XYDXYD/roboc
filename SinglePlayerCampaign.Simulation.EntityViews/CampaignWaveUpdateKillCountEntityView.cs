using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveUpdateKillCountEntityView : EntityView
	{
		public IKillCountComponent killCountComponent;

		public ISpawnDataComponent spawnDataComponent;

		public CampaignWaveUpdateKillCountEntityView()
			: this()
		{
		}
	}
}
