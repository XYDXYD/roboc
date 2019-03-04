using Simulation;
using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.Simulation.Components;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveEnemyDespawnEngine : SingleEntityViewEngine<CampaignWaveEnemyDespawnEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ITaskRoutine _tick;

		private readonly DestructionManager _destructionManager;

		private readonly ICommandFactory _commandFactory;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public CampaignWaveEnemyDespawnEngine(DestructionManager destructionManager, ICommandFactory commandFactory)
		{
			_destructionManager = destructionManager;
			_commandFactory = commandFactory;
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveEnemyDespawnEntityView entityView)
		{
			entityView.timeComponent.timeRunning.NotifyOnValueSet((Action<int, bool>)ToggleTimer);
			SpawnEvent[] spawnEvents = entityView.spawnDataComponent.spawnEvents;
			foreach (SpawnEvent spawnEvent in spawnEvents)
			{
				spawnEvent.robotsKilled.NotifyOnValueSet((Action<int, int>)OnRobotKilled);
			}
		}

		protected override void Remove(CampaignWaveEnemyDespawnEntityView entityView)
		{
			SpawnEvent[] spawnEvents = entityView.spawnDataComponent.spawnEvents;
			foreach (SpawnEvent spawnEvent in spawnEvents)
			{
				spawnEvent.robotsKilled.StopNotify((Action<int, int>)OnRobotKilled);
			}
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

		private void OnRobotKilled(int entityId, int value)
		{
			CampaignWaveEnemyDespawnEntityView campaignWaveEnemyDespawnEntityView = entityViewsDB.QueryEntityView<CampaignWaveEnemyDespawnEntityView>(207);
			int num = campaignWaveEnemyDespawnEntityView.spawnDataComponent.spawnEvents.Length;
			for (int i = 0; i < num; i++)
			{
				WaveData waveData = campaignWaveEnemyDespawnEntityView.waveDataComponent.waveData;
				WaveRobot waveRobot = waveData.WaveRobots[i];
				SpawnEvent spawnEvent = campaignWaveEnemyDespawnEntityView.spawnDataComponent.spawnEvents[i];
				if (!spawnEvent.alreadyDespawned && spawnEvent.robotsKilled.get_value() >= waveRobot.killsToDespawn && waveRobot.killsToDespawn != 0)
				{
					spawnEvent.alreadyDespawned = true;
					DespawnGroup(i);
				}
			}
		}

		private IEnumerator Tick()
		{
			CampaignWaveEnemyDespawnEntityView campaignWave = entityViewsDB.QueryEntityView<CampaignWaveEnemyDespawnEntityView>(207);
			IWaveDataComponent waveDataComponent = campaignWave.waveDataComponent;
			ITimeComponent timeComponent = campaignWave.timeComponent;
			ISpawnDataComponent spawnDataComponent = campaignWave.spawnDataComponent;
			int spawnEventsAmount = spawnDataComponent.spawnEvents.Length;
			while (true)
			{
				for (int i = 0; i < spawnEventsAmount; i++)
				{
					WaveData waveData = waveDataComponent.waveData;
					WaveRobot waveRobot = waveData.WaveRobots[i];
					SpawnEvent spawnEvent = spawnDataComponent.spawnEvents[i];
					if (!spawnEvent.alreadyDespawned && !(timeComponent.elapsedTime < (float)waveRobot.timeToDespawn) && waveRobot.timeToDespawn != 0)
					{
						spawnEvent.alreadyDespawned = true;
						DespawnGroup(i);
					}
				}
				yield return null;
			}
		}

		private void DespawnGroup(int spawnEventId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<AIMachineDespawnEntityView> val = entityViewsDB.QueryEntityViews<AIMachineDespawnEntityView>();
			for (int num = val.get_Count() - 1; num >= 0; num--)
			{
				AIMachineDespawnEntityView aIMachineDespawnEntityView = val.get_Item(num);
				if (aIMachineDespawnEntityView.spawnEventIdComponent.spawnEventId == spawnEventId)
				{
					TaskRunner.get_Instance().Run(Despawn(aIMachineDespawnEntityView.aiBotIdDataComponent.playerId));
				}
			}
		}

		private IEnumerator Despawn(int playerId)
		{
			_destructionManager.DestroyMachine(playerId, playerId);
			yield return null;
			UnregisterAIMachineCommand unregisterAIMachineCommand = _commandFactory.Build<UnregisterAIMachineCommand>();
			unregisterAIMachineCommand.Initialise(playerId, entityViewsDB);
			unregisterAIMachineCommand.Execute();
		}
	}
}
