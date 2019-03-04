using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveUpdateKillCountEngine : SingleEntityViewEngine<CampaignWaveUpdateKillCountEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly DestructionReporter _destructionReporter;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public CampaignWaveUpdateKillCountEngine(DestructionReporter destructionReporter)
		{
			_destructionReporter = destructionReporter;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveUpdateKillCountEntityView entityView)
		{
			_destructionReporter.OnMachineDestroyed += UpdateKillCount;
		}

		protected override void Remove(CampaignWaveUpdateKillCountEntityView entityView)
		{
			_destructionReporter.OnMachineDestroyed -= UpdateKillCount;
		}

		private void UpdateKillCount(int ownerId, int machineId, bool isMe)
		{
			if (isMe)
			{
				return;
			}
			AIMachineKillRequirementEntityView aIMachineKillRequirementEntityView = entityViewsDB.QueryEntityView<AIMachineKillRequirementEntityView>(machineId);
			int spawnEventId = aIMachineKillRequirementEntityView.spawnEventIdComponent.spawnEventId;
			CampaignWaveUpdateKillCountEntityView campaignWaveUpdateKillCountEntityView = entityViewsDB.QueryEntityView<CampaignWaveUpdateKillCountEntityView>(207);
			if (!campaignWaveUpdateKillCountEntityView.spawnDataComponent.spawnEvents[spawnEventId].alreadyDespawned)
			{
				DispatchOnSet<int> robotsKilled = campaignWaveUpdateKillCountEntityView.spawnDataComponent.spawnEvents[spawnEventId].robotsKilled;
				robotsKilled.set_value(robotsKilled.get_value() + 1);
				if (aIMachineKillRequirementEntityView.isKillRequirementComponent.isKillRequirement)
				{
					DispatchOnSet<int> killCount = campaignWaveUpdateKillCountEntityView.killCountComponent.killCount;
					killCount.set_value(killCount.get_value() + 1);
				}
			}
		}
	}
}
