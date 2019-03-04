using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveUpdateTimeEngine : SingleEntityViewEngine<CampaignWaveUpdateTimeEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ITaskRoutine _tick;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public CampaignWaveUpdateTimeEngine()
		{
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveUpdateTimeEntityView entityView)
		{
			entityView.timeComponent.timeRunning.NotifyOnValueSet((Action<int, bool>)ToggleTimer);
		}

		protected override void Remove(CampaignWaveUpdateTimeEntityView entityView)
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
			CampaignWaveUpdateTimeEntityView entity = entityViewsDB.QueryEntityView<CampaignWaveUpdateTimeEntityView>(207);
			while (true)
			{
				entity.timeComponent.elapsedTime += Time.get_deltaTime();
				yield return null;
			}
		}
	}
}
