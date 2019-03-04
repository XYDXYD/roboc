using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;
using System;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignVictoryCheckEngine : SingleEntityViewEngine<CampaignVictoryCheckEntityView>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignVictoryCheckEntityView entityView)
		{
			entityView.currentWaveComponent.counterValue.NotifyOnValueSet((Action<int, int>)OnWaveCompleted);
		}

		protected override void Remove(CampaignVictoryCheckEntityView entityView)
		{
			entityView.currentWaveComponent.counterValue.StopNotify((Action<int, int>)OnWaveCompleted);
		}

		private void OnWaveCompleted(int entityId, int currentWave)
		{
			CampaignVictoryCheckEntityView campaignVictoryCheckEntityView = entityViewsDB.QueryEntityView<CampaignVictoryCheckEntityView>(208);
			if (campaignVictoryCheckEntityView.currentWaveComponent.counterValue.get_value() == campaignVictoryCheckEntityView.currentWaveComponent.maxValue && !campaignVictoryCheckEntityView.campaignVictoryComponent.victoryAchieved.get_value())
			{
				campaignVictoryCheckEntityView.campaignVictoryComponent.victoryAchieved.set_value(true);
			}
		}
	}
}
