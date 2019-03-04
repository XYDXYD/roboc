using SinglePlayerCampaign.Simulation.Components;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityViews
{
	internal class CampaignWaveSpawnSchedulingEntityView : EntityView
	{
		public ITimeComponent timeComponent;

		public IWaveDataComponent waveData;

		public IKillCountComponent killCountComponent;

		public ISpawnDataComponent spawnDataComponent;

		public ISpawnRequestComponent spawnRequestComponent;

		public CampaignWaveSpawnSchedulingEntityView()
			: this()
		{
		}
	}
}
