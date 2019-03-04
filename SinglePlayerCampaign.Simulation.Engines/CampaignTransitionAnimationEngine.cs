using Simulation.SinglePlayerCampaign.DataTypes;
using Simulation.SinglePlayerCampaign.EntityViews;
using SinglePlayerCampaign.GUI.Simulation.EntityViews;
using SinglePlayerCampaign.Simulation.Components;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignTransitionAnimationEngine : SingleEntityViewEngine<CurrentWaveTrackerEntityView>, IQueryingEntityViewEngine, IEngine
	{
		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(CurrentWaveTrackerEntityView entityView)
		{
			entityView.TransitionAnimationTriggerComponent.WaveCompleted.NotifyOnValueSet((Action<int, bool>)HandleWaveCompleted);
			entityView.ReadyToSpawnWaveComponent.ReadyToSpawn.NotifyOnValueSet((Action<int, bool>)HandleReadyToSpawn);
		}

		protected override void Remove(CurrentWaveTrackerEntityView entityView)
		{
			entityView.TransitionAnimationTriggerComponent.WaveCompleted.StopNotify((Action<int, bool>)HandleWaveCompleted);
			entityView.ReadyToSpawnWaveComponent.ReadyToSpawn.StopNotify((Action<int, bool>)HandleReadyToSpawn);
		}

		private void HandleWaveCompleted(int entityID, bool ready)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (ready)
			{
				ICounterComponent currentWaveCounterComponent = entityViewsDB.QueryEntityView<CurrentWaveTrackerEntityView>(entityID).CurrentWaveCounterComponent;
				int value = currentWaveCounterComponent.counterValue.get_value();
				int num = value + 1;
				int maxValue = currentWaveCounterComponent.maxValue;
				if (num != maxValue)
				{
					entityViewsDB.QueryEntityViews<WaveTransitionEntityView>().get_Item(0).AnimationComponent.PlayWaveComplete();
				}
			}
		}

		private void HandleReadyToSpawn(int entityID, bool ready)
		{
			if (ready)
			{
				ICounterComponent currentWaveCounterComponent = entityViewsDB.QueryEntityView<CurrentWaveTrackerEntityView>(entityID).CurrentWaveCounterComponent;
				if (currentWaveCounterComponent.counterValue.get_value() == 0)
				{
					PlayStartBattleAnimation();
				}
				else
				{
					TaskRunner.get_Instance().Run(PlayStartWaveAnimation(currentWaveCounterComponent));
				}
			}
		}

		private IEnumerator PlayStartWaveAnimation(ICounterComponent currentWaveCounterComponent)
		{
			FasterReadOnlyList<WaveTransitionEntityView> waveTransitionsEntityViews = entityViewsDB.QueryEntityViews<WaveTransitionEntityView>();
			while (waveTransitionsEntityViews.get_Item(0).AnimationComponent.IsPlaying)
			{
				yield return null;
			}
			WavesDataEntityView wavesData = entityViewsDB.QueryEntityView<WavesDataEntityView>(204);
			int currentWaveIndex = currentWaveCounterComponent.counterValue.get_value();
			WaveData waveData = wavesData.wavesData.wavesData.WavesData.get_Item(currentWaveIndex);
			bool isBossWave = false;
			WaveRobot[] waveRobots = waveData.WaveRobots;
			for (int i = 0; i < waveRobots.Length; i++)
			{
				WaveRobot waveRobot = waveRobots[i];
				if (waveRobot.isBoss)
				{
					isBossWave = true;
					break;
				}
			}
			waveTransitionsEntityViews.get_Item(0).AnimationComponent.PlayWaveStart(isBossWave);
		}

		private void PlayStartBattleAnimation()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			entityViewsDB.QueryEntityViews<WaveTransitionEntityView>().get_Item(0).AnimationComponent.PlayStartBattle();
		}
	}
}
