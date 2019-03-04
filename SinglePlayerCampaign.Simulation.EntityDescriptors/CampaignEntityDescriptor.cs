using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.EntityDescriptors
{
	internal class CampaignEntityDescriptor : EntityDescriptor
	{
		private static readonly IEntityViewBuilder[] _nodesToBuild;

		static CampaignEntityDescriptor()
		{
			_nodesToBuild = (IEntityViewBuilder[])new IEntityViewBuilder[5]
			{
				new EntityViewBuilder<CampaignUpdateRemainingLivesEntityView>(),
				new EntityViewBuilder<CampaignShowRemainingLivesEntityView>(),
				new EntityViewBuilder<CampaignDefeatCheckEntityView>(),
				new EntityViewBuilder<CurrentWaveTrackerEntityView>(),
				new EntityViewBuilder<CampaignVictoryCheckEntityView>()
			};
		}

		public CampaignEntityDescriptor()
			: this(_nodesToBuild)
		{
		}
	}
}
