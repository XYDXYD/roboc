using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveEnemyDespawnEntityView : EntityView
	{
		public ISpawnDataComponent spawnDataComponent;

		public ITimeComponent timeComponent;

		public IWaveDataComponent waveDataComponent;

		public CampaignWaveEnemyDespawnEntityView()
			: this()
		{
		}
	}
}
