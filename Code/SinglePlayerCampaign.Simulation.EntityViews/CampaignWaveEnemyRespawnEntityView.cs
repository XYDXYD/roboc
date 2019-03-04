using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveEnemyRespawnEntityView : EntityView
	{
		public IWaveDataComponent waveData;

		public ITimeComponent timeComponent;

		public ISpawnDataComponent spawnDataComponent;

		public CampaignWaveEnemyRespawnEntityView()
			: this()
		{
		}
	}
}
