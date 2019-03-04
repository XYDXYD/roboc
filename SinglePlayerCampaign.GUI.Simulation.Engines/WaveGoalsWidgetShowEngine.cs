using SinglePlayerCampaign.GUI.Simulation.EntityViews;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;

namespace SinglePlayerCampaign.GUI.Simulation.Engines
{
	internal class WaveGoalsWidgetShowEngine : MultiEntityViewsEngine<CampaignWaveShowGoalsEntityView, WaveGoalsWidgetShowEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ITaskRoutine _tick;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public WaveGoalsWidgetShowEngine()
		{
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveShowGoalsEntityView entityView)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			entityView.killCountComponent.killCount.NotifyOnValueSet((Action<int, int>)UpdateEnemiesNumber);
			if (IsTimerActive(entityView))
			{
				_tick.Start((Action<PausableTaskException>)null, (Action)null);
			}
			FasterListEnumerator<WaveGoalsWidgetShowEntityView> enumerator = entityViewsDB.QueryEntityViews<WaveGoalsWidgetShowEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					WaveGoalsWidgetShowEntityView current = enumerator.get_Current();
					InitialiseWidget(entityView, current);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		protected override void Remove(CampaignWaveShowGoalsEntityView entityView)
		{
			entityView.killCountComponent.killCount.StopNotify((Action<int, int>)UpdateEnemiesNumber);
			if (IsTimerActive(entityView))
			{
				_tick.Stop();
			}
		}

		protected override void Add(WaveGoalsWidgetShowEntityView entityView)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CampaignWaveShowGoalsEntityView> val = entityViewsDB.QueryEntityViews<CampaignWaveShowGoalsEntityView>();
			if (val.get_Count() != 0)
			{
				InitialiseWidget(val.get_Item(0), entityView);
			}
		}

		protected override void Remove(WaveGoalsWidgetShowEntityView entityView)
		{
		}

		private static void InitialiseWidget(CampaignWaveShowGoalsEntityView data, WaveGoalsWidgetShowEntityView view)
		{
			if (data.waveDefeatComponent.timeLimit > 0f)
			{
				view.timedEliminationComponent.remainingEnemies = GetRemainingEnemies(data);
				view.timedEliminationComponent.timeLeft = FormatTime(data.waveDefeatComponent.timeLimit);
				view.timedEliminationComponent.isActive = true;
				view.survivalComponent.isActive = false;
				view.eliminationComponent.isActive = false;
			}
			else if (data.waveVictoryComponent.timeRequired > 0f)
			{
				view.survivalComponent.timeLeft = FormatTime(data.waveVictoryComponent.timeRequired);
				view.timedEliminationComponent.isActive = false;
				view.survivalComponent.isActive = true;
				view.eliminationComponent.isActive = false;
			}
			else
			{
				view.eliminationComponent.remainingEnemies = GetRemainingEnemies(data);
				view.timedEliminationComponent.isActive = false;
				view.survivalComponent.isActive = false;
				view.eliminationComponent.isActive = true;
			}
		}

		private void UpdateEnemiesNumber(int entityId, int killCount)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			CampaignWaveShowGoalsEntityView campaignWaveShowGoalsEntityView = entityViewsDB.QueryEntityView<CampaignWaveShowGoalsEntityView>(207);
			FasterListEnumerator<WaveGoalsWidgetShowEntityView> enumerator = entityViewsDB.QueryEntityViews<WaveGoalsWidgetShowEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					WaveGoalsWidgetShowEntityView current = enumerator.get_Current();
					if (campaignWaveShowGoalsEntityView.waveDefeatComponent.timeLimit > 0f)
					{
						current.timedEliminationComponent.remainingEnemies = GetRemainingEnemies(campaignWaveShowGoalsEntityView);
					}
					else
					{
						current.eliminationComponent.remainingEnemies = GetRemainingEnemies(campaignWaveShowGoalsEntityView);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private IEnumerator Tick()
		{
			CampaignWaveShowGoalsEntityView data = entityViewsDB.QueryEntityView<CampaignWaveShowGoalsEntityView>(207);
			while (true)
			{
				FasterListEnumerator<WaveGoalsWidgetShowEntityView> enumerator = entityViewsDB.QueryEntityViews<WaveGoalsWidgetShowEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						WaveGoalsWidgetShowEntityView current = enumerator.get_Current();
						if (data.waveDefeatComponent.timeLimit > 0f)
						{
							float time = data.waveDefeatComponent.timeLimit - data.timeComponent.elapsedTime;
							current.timedEliminationComponent.timeLeft = FormatTime(time);
						}
						else
						{
							float time2 = data.waveVictoryComponent.timeRequired - data.timeComponent.elapsedTime;
							current.survivalComponent.timeLeft = FormatTime(time2);
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				yield return null;
			}
		}

		private static bool IsTimerActive(CampaignWaveShowGoalsEntityView entityView)
		{
			return entityView.waveVictoryComponent.timeRequired > 0f || entityView.waveDefeatComponent.timeLimit > 0f;
		}

		private static int GetRemainingEnemies(CampaignWaveShowGoalsEntityView showRemainingEnemiesEntityView)
		{
			int num = showRemainingEnemiesEntityView.waveVictoryComponent.killsRequired - showRemainingEnemiesEntityView.killCountComponent.killCount.get_value();
			if (num < 0)
			{
				num = 0;
			}
			return num;
		}

		private static string FormatTime(float time)
		{
			if (time < 0f)
			{
				time = 0f;
			}
			TimeSpan timeSpan = TimeSpan.FromSeconds(time);
			return $"{timeSpan.Minutes:0}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds:000}";
		}
	}
}
