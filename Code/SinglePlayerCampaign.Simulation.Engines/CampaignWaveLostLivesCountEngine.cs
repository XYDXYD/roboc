using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveLostLivesCountEngine : SingleEntityViewEngine<CampaignWaveLostLivesEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly DestructionReporter _destructionReporter;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		internal CampaignWaveLostLivesCountEngine(DestructionReporter destructionReporter)
		{
			_destructionReporter = destructionReporter;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveLostLivesEntityView entityView)
		{
			_destructionReporter.OnMachineDestroyed += UpdateKillCount;
		}

		protected override void Remove(CampaignWaveLostLivesEntityView entityView)
		{
			_destructionReporter.OnMachineDestroyed -= UpdateKillCount;
		}

		private void UpdateKillCount(int ownerId, int machineId, bool isMe)
		{
			if (isMe)
			{
				CampaignWaveLostLivesEntityView campaignWaveLostLivesEntityView = entityViewsDB.QueryEntityView<CampaignWaveLostLivesEntityView>(207);
				DispatchOnSet<int> counterValue = campaignWaveLostLivesEntityView.currentWaveLostLivesComponent.counterValue;
				counterValue.set_value(counterValue.get_value() + 1);
			}
		}
	}
}
