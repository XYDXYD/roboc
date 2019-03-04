using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveDefeatCheckEngine : SingleEntityViewEngine<CampaignWaveDefeatCheckEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ITaskRoutine _tick;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public CampaignWaveDefeatCheckEngine()
		{
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveDefeatCheckEntityView entityView)
		{
			entityView.timeComponent.timeRunning.NotifyOnValueSet((Action<int, bool>)ToggleTimer);
		}

		protected override void Remove(CampaignWaveDefeatCheckEntityView entityView)
		{
			entityView.timeComponent.timeRunning.StopNotify((Action<int, bool>)ToggleTimer);
		}

		private void ToggleTimer(int entityId, bool timeRunning)
		{
			if (timeRunning)
			{
				_tick.Start((Action<PausableTaskException>)null, (Action)null);
			}
			else
			{
				_tick.Stop();
			}
		}

		private IEnumerator Tick()
		{
			CampaignWaveDefeatCheckEntityView entity = entityViewsDB.QueryEntityView<CampaignWaveDefeatCheckEntityView>(207);
			while (!(entity.timeComponent.elapsedTime > entity.waveDefeatComponent.timeLimit) || !(entity.waveDefeatComponent.timeLimit > 0f) || entity.waveDefeatComponent.defeatHappened.get_value())
			{
				yield return null;
			}
			entity.waveDefeatComponent.defeatHappened.set_value(true);
		}
	}
}
