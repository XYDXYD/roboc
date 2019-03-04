using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignUpdateRemainingLivesEngine : SingleEntityViewEngine<CampaignUpdateRemainingLivesEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly DestructionReporter _destructionReporter;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public CampaignUpdateRemainingLivesEngine(DestructionReporter destructionReporter)
		{
			_destructionReporter = destructionReporter;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignUpdateRemainingLivesEntityView entityView)
		{
			_destructionReporter.OnMachineDestroyed += UpdateKillCount;
		}

		protected override void Remove(CampaignUpdateRemainingLivesEntityView entityView)
		{
			_destructionReporter.OnMachineDestroyed -= UpdateKillCount;
		}

		private void UpdateKillCount(int ownerId, int machineId, bool isMe)
		{
			if (isMe)
			{
				CampaignUpdateRemainingLivesEntityView campaignUpdateRemainingLivesEntityView = entityViewsDB.QueryEntityView<CampaignUpdateRemainingLivesEntityView>(208);
				DispatchOnSet<int> remainingLives = campaignUpdateRemainingLivesEntityView.remainingLivesComponent.remainingLives;
				remainingLives.set_value(remainingLives.get_value() - 1);
			}
		}
	}
}
