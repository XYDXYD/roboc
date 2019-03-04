using Simulation;
using Simulation.SinglePlayerCampaign.EntityViews;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.DataStructures;
using Svelto.ECS;
using System;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal sealed class CampaignHealthBoostEngine : MultiEntityViewsEngine<AIMachineEntityView, WavesDataEntityView, CurrentWaveTrackerEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly FasterList<AIMachineEntityView> _waitingMachines = new FasterList<AIMachineEntityView>();

		private readonly HealthTracker _healthTracker;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public CampaignHealthBoostEngine(HealthTracker healthTracker_)
		{
			_healthTracker = healthTracker_;
		}

		public void Ready()
		{
		}

		protected override void Add(AIMachineEntityView entityView)
		{
			_waitingMachines.Add(entityView);
			ApplyBoost();
		}

		protected override void Remove(AIMachineEntityView entityView)
		{
			if (_waitingMachines.Contains(entityView))
			{
				_waitingMachines.Remove(entityView);
			}
		}

		protected override void Add(WavesDataEntityView entityView)
		{
			ApplyBoost();
		}

		protected override void Remove(WavesDataEntityView entityView)
		{
		}

		protected override void Add(CurrentWaveTrackerEntityView entityView)
		{
			ApplyBoost();
		}

		protected override void Remove(CurrentWaveTrackerEntityView entityView)
		{
		}

		private void ApplyBoost()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<WavesDataEntityView> val = entityViewsDB.QueryEntityViews<WavesDataEntityView>();
			FasterReadOnlyList<CurrentWaveTrackerEntityView> val2 = entityViewsDB.QueryEntityViews<CurrentWaveTrackerEntityView>();
			if (val.get_Count() != 0 && val2.get_Count() != 0)
			{
				WavesDataEntityView wavesDataEntityView = val.get_Item(0);
				CurrentWaveTrackerEntityView currentWaveTrackerEntityView = val2.get_Item(0);
				FasterListEnumerator<AIMachineEntityView> enumerator = _waitingMachines.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						AIMachineEntityView current = enumerator.get_Current();
						FasterList<InstantiatedCube> allInstantiatedCubes = current.machineMapComponent.machineMap.GetAllInstantiatedCubes();
						int num = 0;
						FasterListEnumerator<InstantiatedCube> enumerator2 = allInstantiatedCubes.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								InstantiatedCube current2 = enumerator2.get_Current();
								CampaignDifficultySetting campaignDifficulty = wavesDataEntityView.wavesData.wavesData.CampaignDifficulty;
								EnemySetting enemyDifficultySettings = campaignDifficulty.EnemyDifficultySettings;
								float initialHealthBoost = enemyDifficultySettings.InitialHealthBoost;
								CampaignDifficultySetting campaignDifficulty2 = wavesDataEntityView.wavesData.wavesData.CampaignDifficulty;
								EnemySetting enemyDifficultySettings2 = campaignDifficulty2.EnemyDifficultySettings;
								float increasePerWaveHealthBoost = enemyDifficultySettings2.IncreasePerWaveHealthBoost;
								int value = currentWaveTrackerEntityView.CurrentWaveCounterComponent.counterValue.get_value();
								float num2 = 1f + initialHealthBoost + (float)value * increasePerWaveHealthBoost;
								current2.health = (int)((float)current2.health * num2);
								current2.initialTotalHealth = Math.Max(1, (int)((float)current2.initialTotalHealth * num2));
								num += current2.initialTotalHealth;
							}
						}
						finally
						{
							((IDisposable)enumerator2).Dispose();
						}
						_healthTracker.ApplyCampaignHealthBoost(current.get_ID(), num);
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				_waitingMachines.Clear();
			}
		}
	}
}
