using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveVictoryCheckEngine : SingleEntityViewEngine<CampaignWaveVictoryCheckEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ITaskRoutine _tick;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public CampaignWaveVictoryCheckEngine()
		{
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveVictoryCheckEntityView entityView)
		{
			entityView.killCountComponent.killCount.NotifyOnValueSet((Action<int, int>)OnKill);
			_tick.Start((Action<PausableTaskException>)null, (Action)null);
		}

		protected override void Remove(CampaignWaveVictoryCheckEntityView entityView)
		{
			entityView.killCountComponent.killCount.StopNotify((Action<int, int>)OnKill);
			_tick.Stop();
		}

		private void OnKill(int entityId, int killCount)
		{
			CampaignWaveVictoryCheckEntityView entity = entityViewsDB.QueryEntityView<CampaignWaveVictoryCheckEntityView>(207);
			CheckForVictory(entity);
		}

		private IEnumerator Tick()
		{
			CampaignWaveVictoryCheckEntityView campaignWaveVictoryCheckEntityView = entityViewsDB.QueryEntityView<CampaignWaveVictoryCheckEntityView>(207);
			while (!(campaignWaveVictoryCheckEntityView.timeComponent.elapsedTime > campaignWaveVictoryCheckEntityView.waveVictoryComponent.timeRequired))
			{
				yield return null;
			}
			CheckForVictory(campaignWaveVictoryCheckEntityView);
		}

		private static void CheckForVictory(CampaignWaveVictoryCheckEntityView entity)
		{
			if (entity.killCountComponent.killCount.get_value() >= entity.waveVictoryComponent.killsRequired && !(entity.timeComponent.elapsedTime < entity.waveVictoryComponent.timeRequired) && !entity.waveVictoryComponent.victoryAchieved.get_value())
			{
				entity.waveVictoryComponent.victoryAchieved.set_value(true);
			}
		}
	}
}
