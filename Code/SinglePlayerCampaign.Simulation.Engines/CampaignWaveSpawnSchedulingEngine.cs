using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.Simulation.Components;
using SinglePlayerCampaign.Simulation.EntityViews;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SinglePlayerCampaign.Simulation.Engines
{
	internal class CampaignWaveSpawnSchedulingEngine : SingleEntityViewEngine<CampaignWaveSpawnSchedulingEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly ITaskRoutine _tick;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public CampaignWaveSpawnSchedulingEngine()
		{
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public void Ready()
		{
		}

		protected override void Add(CampaignWaveSpawnSchedulingEntityView entityView)
		{
			entityView.timeComponent.timeRunning.NotifyOnValueSet((Action<int, bool>)ToggleTimer);
		}

		protected override void Remove(CampaignWaveSpawnSchedulingEntityView entityView)
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
			CampaignWaveSpawnSchedulingEntityView campaignWave = entityViewsDB.QueryEntityView<CampaignWaveSpawnSchedulingEntityView>(207);
			IWaveDataComponent waveDataComponent = campaignWave.waveData;
			IKillCountComponent killCountComponent = campaignWave.killCountComponent;
			ITimeComponent timeComponent = campaignWave.timeComponent;
			ISpawnDataComponent spawnDataComponent = campaignWave.spawnDataComponent;
			ISpawnRequestComponent spawnRequestComponent = campaignWave.spawnRequestComponent;
			List<SpawnRequest> spawnRequests = new List<SpawnRequest>();
			while (true)
			{
				int spawnEventsAmount = spawnDataComponent.spawnEvents.Length;
				for (int i = 0; i < spawnEventsAmount; i++)
				{
					WaveData waveData = waveDataComponent.waveData;
					WaveRobot robot = waveData.WaveRobots[i];
					SpawnEvent spawnEvent = spawnDataComponent.spawnEvents[i];
					if (!spawnEvent.alreadyDespawned && killCountComponent.killCount.get_value() >= robot.killsToSpawn && spawnEvent.robotsSpawned < robot.maxRobotAmount && !(timeComponent.elapsedTime < spawnEvent.timeOfNextSpawn))
					{
						int num = robot.periodicRobotAmount;
						if (!spawnEvent.initialRobotsSpawned)
						{
							num = robot.initialRobotAmount;
							spawnEvent.initialRobotsSpawned = true;
						}
						int num2 = robot.maxRobotAmount - spawnEvent.robotsSpawned;
						if (num > num2)
						{
							num = num2;
						}
						spawnRequests.Clear();
						for (int j = 0; j < num; j++)
						{
							spawnRequests.Add(new SpawnRequest(RobotNameHelper.GetName(robot, i, spawnEvent.robotsSpawned + j), i, robot.isBoss, robot.isKillRequirement));
						}
						spawnRequestComponent.spawnRequests.set_value((IEnumerable<SpawnRequest>)spawnRequests);
						spawnEvent.robotsSpawned += num;
						spawnEvent.timeOfNextSpawn += robot.spawnInterval;
					}
				}
				yield return null;
			}
		}
	}
}
