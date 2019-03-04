using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.EntityViews
{
	internal class CampaignWaveEnemySpawnEntityView : EntityView
	{
		public ISpawnRequestComponent spawnRequestComponent;

		public CampaignWaveEnemySpawnEntityView()
			: this()
		{
		}
	}
}
