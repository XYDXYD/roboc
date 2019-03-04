using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;
using System;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignDefeatCheckEngine : MultiEntityViewsEngine<CampaignDefeatCheckEntityView, CampaignWaveDefeatCheckEntityView>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignDefeatCheckEntityView entityView)
		{
			entityView.remainingLivesComponent.remainingLives.NotifyOnValueSet((Action<int, int>)OnDeath);
		}

		protected override void Remove(CampaignDefeatCheckEntityView entityView)
		{
			entityView.remainingLivesComponent.remainingLives.StopNotify((Action<int, int>)OnDeath);
		}

		protected override void Add(CampaignWaveDefeatCheckEntityView entityView)
		{
			entityView.waveDefeatComponent.defeatHappened.NotifyOnValueSet((Action<int, bool>)OnWaveDefeat);
		}

		protected override void Remove(CampaignWaveDefeatCheckEntityView entityView)
		{
			entityView.waveDefeatComponent.defeatHappened.StopNotify((Action<int, bool>)OnWaveDefeat);
		}

		private void OnDeath(int entityId, int remainingLives)
		{
			if (remainingLives <= 0)
			{
				OnDefeat();
			}
		}

		private void OnWaveDefeat(int entityId, bool defeatHappened)
		{
			if (defeatHappened)
			{
				OnDefeat();
			}
		}

		private void OnDefeat()
		{
			CampaignDefeatCheckEntityView campaignDefeatCheckEntityView = entityViewsDB.QueryEntityView<CampaignDefeatCheckEntityView>(208);
			if (!campaignDefeatCheckEntityView.campaignDefeatComponent.defeatHappened.get_value())
			{
				campaignDefeatCheckEntityView.campaignDefeatComponent.defeatHappened.set_value(true);
			}
		}
	}
}
