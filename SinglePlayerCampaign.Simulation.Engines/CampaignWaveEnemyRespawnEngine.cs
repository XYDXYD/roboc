using Simulation;
using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveEnemyRespawnEngine : SingleEntityViewEngine<CampaignWaveEnemyRespawnEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private const float RESPAWN_TIME = 1f;

		private readonly DestructionReporter _destructionReporter;

		private readonly RespawnAtPositionClientCommand _respawnAtPositionClientCommand;

		private readonly ISpawnPointManager _spawnPointManager;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public CampaignWaveEnemyRespawnEngine(DestructionReporter destructionReporter, RespawnAtPositionClientCommand respawnAtPositionClientCommand, ISpawnPointManager spawnPointManager)
		{
			_destructionReporter = destructionReporter;
			_respawnAtPositionClientCommand = respawnAtPositionClientCommand;
			_spawnPointManager = spawnPointManager;
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveEnemyRespawnEntityView entityView)
		{
			entityView.timeComponent.timeRunning.NotifyOnValueSet((Action<int, bool>)OnSetTimeRunning);
		}

		protected override void Remove(CampaignWaveEnemyRespawnEntityView entityView)
		{
			entityView.timeComponent.timeRunning.StopNotify((Action<int, bool>)OnSetTimeRunning);
		}

		private void OnSetTimeRunning(int entityId, bool timeRunning)
		{
			if (timeRunning)
			{
				_destructionReporter.OnMachineDestroyed += OnMachineDestroyed;
			}
			else
			{
				_destructionReporter.OnMachineDestroyed -= OnMachineDestroyed;
			}
		}

		private void OnMachineDestroyed(int ownerId, int machineId, bool isMe)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (isMe)
			{
				return;
			}
			AIMachineRespawnEntityView aIMachineRespawnEntityView = entityViewsDB.QueryEntityView<AIMachineRespawnEntityView>(machineId);
			int spawnEventId = aIMachineRespawnEntityView.spawnEventIdComponent.spawnEventId;
			CampaignWaveEnemyRespawnEntityView campaignWaveEnemyRespawnEntityView = entityViewsDB.QueryEntityView<CampaignWaveEnemyRespawnEntityView>(207);
			if (!campaignWaveEnemyRespawnEntityView.spawnDataComponent.spawnEvents[spawnEventId].alreadyDespawned)
			{
				FasterReadOnlyList<AIMachineRespawnEntityView> val = entityViewsDB.QueryEntityViews<AIMachineRespawnEntityView>();
				int num = 0;
				FasterListEnumerator<AIMachineRespawnEntityView> enumerator = val.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						AIMachineRespawnEntityView current = enumerator.get_Current();
						if (current.aliveStateComponent.isAlive.get_value() && current.spawnEventIdComponent.spawnEventId == spawnEventId)
						{
							num++;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				int num2 = num;
				WaveData waveData = campaignWaveEnemyRespawnEntityView.waveData.waveData;
				if (num2 < waveData.WaveRobots[spawnEventId].minRobotAmount)
				{
					TaskRunner.get_Instance().Run(RespawnMachine(ownerId, machineId, campaignWaveEnemyRespawnEntityView));
				}
			}
		}

		private IEnumerator RespawnMachine(int ownerId, int machineId, CampaignWaveEnemyRespawnEntityView campaignWaveEnemyRespawnEntityView)
		{
			yield return (object)new WaitForSecondsEnumerator(1f);
			if (campaignWaveEnemyRespawnEntityView.timeComponent.timeRunning.get_value())
			{
				SpawningPoint spawningPoint = _spawnPointManager.GetNextFreeSpawnPoint(SpawnPoints.SpawnPointsType.PitModeStartLocations, ownerId);
				SpawnPointDependency spawnPointDependency = new SpawnPointDependency(spawningPoint.get_transform().get_position(), spawningPoint.get_transform().get_rotation(), machineId);
				_respawnAtPositionClientCommand.Inject(spawnPointDependency);
				_respawnAtPositionClientCommand.Execute();
			}
		}
	}
}
