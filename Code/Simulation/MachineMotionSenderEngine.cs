using Svelto.ECS;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal class MachineMotionSenderEngine : IEngine, IQueryingEntityViewEngine
	{
		private const float SEND_INTERVAL = 0.1f;

		private GameStateClient _gameStateClient;

		private MachineSpawnDispatcher _spawnDispatcher;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public IDataSenderClientNetworkSpecific dataSender
		{
			private get;
			set;
		}

		public MachineMotionSenderEngine(MachineSpawnDispatcher spawnDispatcher, GameStateClient gameStateClient)
		{
			_gameStateClient = gameStateClient;
			spawnDispatcher.OnPlayerSpawnedIn += OnPlayerSpawnIn;
			spawnDispatcher.OnPlayerRespawnedIn += OnPlayerSpawnIn;
			_spawnDispatcher = spawnDispatcher;
		}

		public void Ready()
		{
		}

		private void OnPlayerSpawnIn(SpawnInParametersPlayer p)
		{
			int num = default(int);
			MachineMotionSenderEntityView[] array = entityViewsDB.QueryEntityViewsAsArray<MachineMotionSenderEntityView>(ref num);
			int num2 = 0;
			while (true)
			{
				if (num2 < num)
				{
					MachineMotionSenderEntityView machineMotionSenderEntityView = array[num2];
					if (machineMotionSenderEntityView.ownerComponent.ownerId == p.playerId)
					{
						break;
					}
					num2++;
					continue;
				}
				return;
			}
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
			_spawnDispatcher.OnPlayerSpawnedIn -= OnPlayerSpawnIn;
			_spawnDispatcher.OnPlayerRespawnedIn -= OnPlayerSpawnIn;
		}

		private IEnumerator Tick()
		{
			WaitForSecondsEnumerator wait = new WaitForSecondsEnumerator(0.1f);
			while (!_gameStateClient.hasGameEnded)
			{
				int count = default(int);
				MachineMotionSenderEntityView[] machines = entityViewsDB.QueryEntityViewsAsArray<MachineMotionSenderEntityView>(ref count);
				for (int i = 0; i < count; i++)
				{
					MachineMotionSenderEntityView machineMotionSenderEntityView = machines[i];
					if (machineMotionSenderEntityView.aliveStateComponent.isAlive.get_value())
					{
						MachineMotionDependency depenency = new MachineMotionDependency(machineMotionSenderEntityView.ownerComponent.ownerId, machineMotionSenderEntityView.rigidBodyComponent.rb, machineMotionSenderEntityView.weaponRaycastComponent.weaponRaycast.aimPoint, Time.get_time());
						dataSender.SendDataToServer(depenency);
					}
				}
				yield return wait;
			}
		}
	}
}
