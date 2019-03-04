using Simulation.Hardware.Weapons;
using Simulation.SinglePlayerCampaign.EntityViews;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.DataStructures;
using Svelto.ECS;
using System;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal sealed class CampaignDamageBoostEngine : MultiEntityViewsEngine<DamageMultiplierNode, WavesDataEntityView, CurrentWaveTrackerEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly FasterList<DamageMultiplierNode> _waitingWeapons = new FasterList<DamageMultiplierNode>();

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(DamageMultiplierNode node)
		{
			if (node.ownerComponent.ownedByAi)
			{
				_waitingWeapons.Add(node);
				ApplyBoost();
			}
		}

		protected override void Remove(DamageMultiplierNode node)
		{
			if (_waitingWeapons.Contains(node))
			{
				_waitingWeapons.Remove(node);
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
			FasterReadOnlyList<WavesDataEntityView> val = entityViewsDB.QueryEntityViews<WavesDataEntityView>();
			FasterReadOnlyList<CurrentWaveTrackerEntityView> val2 = entityViewsDB.QueryEntityViews<CurrentWaveTrackerEntityView>();
			if (val.get_Count() != 0 && val2.get_Count() != 0)
			{
				WavesDataEntityView wavesDataEntityView = val.get_Item(0);
				CurrentWaveTrackerEntityView currentWaveTrackerEntityView = val2.get_Item(0);
				FasterListEnumerator<DamageMultiplierNode> enumerator = _waitingWeapons.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						DamageMultiplierNode current = enumerator.get_Current();
						CampaignDifficultySetting campaignDifficulty = wavesDataEntityView.wavesData.wavesData.CampaignDifficulty;
						EnemySetting enemyDifficultySettings = campaignDifficulty.EnemyDifficultySettings;
						float initialDamageBoost = enemyDifficultySettings.InitialDamageBoost;
						CampaignDifficultySetting campaignDifficulty2 = wavesDataEntityView.wavesData.wavesData.CampaignDifficulty;
						EnemySetting enemyDifficultySettings2 = campaignDifficulty2.EnemyDifficultySettings;
						float increasePerWaveDamageBoost = enemyDifficultySettings2.IncreasePerWaveDamageBoost;
						int value = currentWaveTrackerEntityView.CurrentWaveCounterComponent.counterValue.get_value();
						float num = 1f + initialDamageBoost + (float)value * increasePerWaveDamageBoost;
						if (num < 0f)
						{
							throw new Exception($"Damage for campaign bots is negative! difficulty factor = {num}");
						}
						current.damageStats.campaignDifficultyFactor = num;
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				_waitingWeapons.Clear();
			}
		}
	}
}
