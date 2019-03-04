using SinglePlayerCampaign.EntityViews;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityDescriptors
{
	internal class CampaignWaveEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static CampaignWaveEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[10]
			{
				new EntityViewBuilder<CampaignWaveUpdateKillCountEntityView>(),
				new EntityViewBuilder<CampaignWaveShowGoalsEntityView>(),
				new EntityViewBuilder<CampaignWaveUpdateTimeEntityView>(),
				new EntityViewBuilder<CampaignWaveVictoryCheckEntityView>(),
				new EntityViewBuilder<CampaignWaveDefeatCheckEntityView>(),
				new EntityViewBuilder<CampaignWaveSpawnSchedulingEntityView>(),
				new EntityViewBuilder<CampaignWaveEnemySpawnEntityView>(),
				new EntityViewBuilder<CampaignWaveEnemyRespawnEntityView>(),
				new EntityViewBuilder<CampaignWaveLostLivesEntityView>(),
				new EntityViewBuilder<CampaignWaveEnemyDespawnEntityView>()
			};
		}

		public CampaignWaveEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
