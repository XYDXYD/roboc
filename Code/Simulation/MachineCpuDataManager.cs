using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class MachineCpuDataManager : IInitialize, IWaitForFrameworkDestruction
	{
		private Dictionary<int, uint> _currentCPUPerMachine = new Dictionary<int, uint>();

		private Dictionary<int, uint> _initialCPUPerMachine = new Dictionary<int, uint>();

		private Dictionary<int, float> _damageCubesModifier = new Dictionary<int, float>();

		private Dictionary<int, HashSet<InstantiatedCube>> _damagedCubes = new Dictionary<int, HashSet<InstantiatedCube>>();

		private float _healthTreshold;

		[Inject]
		internal MachineSpawnDispatcher machineSpawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public event Action<int, uint> OnMachineCpuInitialized = delegate
		{
		};

		public event Action<int, TargetType, int, float> OnMachineCpuChanged = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			machineSpawnDispatcher.OnPlayerRegistered += HandleOnMachineRegistered;
			machineSpawnDispatcher.OnPlayerRespawnedIn += HandleOnPlayerRespawnedIn;
			LoadGameClientSettings();
		}

		private void LoadGameClientSettings()
		{
			IGetGameClientSettingsRequest getGameClientSettingsRequest = serviceFactory.Create<IGetGameClientSettingsRequest>();
			getGameClientSettingsRequest.SetAnswer(new ServiceAnswer<GameClientSettingsDependency>(delegate(GameClientSettingsDependency data)
			{
				_healthTreshold = data.healthThreshold;
			}, delegate(ServiceBehaviour behaviour)
			{
				ErrorWindow.ShowServiceErrorWindow(behaviour);
			}));
			getGameClientSettingsRequest.Execute();
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineSpawnDispatcher.OnPlayerRegistered -= HandleOnMachineRegistered;
			machineSpawnDispatcher.OnPlayerRespawnedIn -= HandleOnPlayerRespawnedIn;
		}

		public uint GetCurrentCpu(int machineId)
		{
			return _currentCPUPerMachine[machineId];
		}

		public float GetCurrentDisplayedHealthPercent(int machineId)
		{
			float num = ((float)(double)_currentCPUPerMachine[machineId] - _damageCubesModifier[machineId]) / (float)(double)_initialCPUPerMachine[machineId];
			num = (num - _healthTreshold) / (1f - _healthTreshold);
			return Mathf.Max(0f, num);
		}

		private void HandleOnMachineRegistered(SpawnInParametersPlayer spawnInParameters)
		{
			int machineId = spawnInParameters.preloadedMachine.machineId;
			uint totalCpu = spawnInParameters.preloadedMachine.machineInfo.totalCpu;
			_currentCPUPerMachine[machineId] = totalCpu;
			_initialCPUPerMachine[machineId] = totalCpu;
			_damageCubesModifier[machineId] = 0f;
			if (!_damagedCubes.ContainsKey(machineId))
			{
				_damagedCubes[machineId] = new HashSet<InstantiatedCube>();
			}
			else
			{
				_damagedCubes[machineId].Clear();
			}
			this.OnMachineCpuInitialized(machineId, totalCpu);
		}

		private void HandleOnPlayerRespawnedIn(SpawnInParametersPlayer player)
		{
			_currentCPUPerMachine[player.machineId] = _initialCPUPerMachine[player.machineId];
			_damageCubesModifier[player.machineId] = 0f;
			_damagedCubes[player.machineId].Clear();
			this.OnMachineCpuInitialized(player.machineId, _currentCPUPerMachine[player.machineId]);
		}

		internal void UpdateOnPlayerCubesRespawned(int shooterId, TargetType shooterType, int machineId, FasterList<InstantiatedCube> respawnedCubes, FasterList<InstantiatedCube> healedCubes)
		{
			AddCPU(machineId, respawnedCubes);
			if (healedCubes.get_Count() > 0)
			{
				HashSet<InstantiatedCube> hashSet = _damagedCubes[machineId];
				for (int i = 0; i < healedCubes.get_Count(); i++)
				{
					InstantiatedCube instantiatedCube = healedCubes.get_Item(i);
					int totalHealth = instantiatedCube.totalHealth;
					if (instantiatedCube.health == totalHealth)
					{
						hashSet.Remove(instantiatedCube);
					}
				}
				CalculateCurrentCubeDamage(machineId, hashSet);
			}
			this.OnMachineCpuChanged(shooterId, shooterType, machineId, GetCurrentDisplayedHealthPercent(machineId));
		}

		private void AddCPU(int playerId, FasterList<InstantiatedCube> cubes)
		{
			uint num = 0u;
			for (int i = 0; i < cubes.get_Count(); i++)
			{
				num += cubes.get_Item(i).persistentCubeData.cpuRating;
			}
			Dictionary<int, uint> currentCPUPerMachine;
			int key;
			(currentCPUPerMachine = _currentCPUPerMachine)[key = playerId] = currentCPUPerMachine[key] + num;
		}

		internal void UpdateOnPlayerCubesDestroyed(int shooterId, int machineId, FasterList<InstantiatedCube> destroyedCubes, FasterList<InstantiatedCube> damagedCubes)
		{
			RemoveCpu(machineId, destroyedCubes);
			if (destroyedCubes.get_Count() > 0)
			{
				HashSet<InstantiatedCube> hashSet = _damagedCubes[machineId];
				for (int i = 0; i < destroyedCubes.get_Count(); i++)
				{
					hashSet.Remove(destroyedCubes.get_Item(i));
				}
			}
			if (destroyedCubes.get_Count() > 0 || damagedCubes.get_Count() > 0)
			{
				HashSet<InstantiatedCube> hashSet2 = _damagedCubes[machineId];
				for (int j = 0; j < damagedCubes.get_Count(); j++)
				{
					hashSet2.Add(damagedCubes.get_Item(j));
				}
				CalculateCurrentCubeDamage(machineId, hashSet2);
			}
			this.OnMachineCpuChanged(shooterId, TargetType.Player, machineId, GetCurrentDisplayedHealthPercent(machineId));
		}

		private void CalculateCurrentCubeDamage(int playerId, HashSet<InstantiatedCube> currentDamagedCubes)
		{
			float num = 0f;
			HashSet<InstantiatedCube>.Enumerator enumerator = currentDamagedCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				num += CalculateDamagedCpu(enumerator.Current);
			}
			_damageCubesModifier[playerId] = num;
		}

		private static float CalculateDamagedCpu(InstantiatedCube cube)
		{
			float num = cube.totalHealth;
			float num2 = cube.health;
			float num3 = 1f - num2 / num;
			return num3 * (float)(double)cube.persistentCubeData.cpuRating;
		}

		private void RemoveCpu(int playerId, FasterList<InstantiatedCube> cubes)
		{
			uint num = 0u;
			for (int i = 0; i < cubes.get_Count(); i++)
			{
				num += cubes.get_Item(i).persistentCubeData.cpuRating;
			}
			Dictionary<int, uint> currentCPUPerMachine;
			int key;
			(currentCPUPerMachine = _currentCPUPerMachine)[key = playerId] = currentCPUPerMachine[key] - num;
		}
	}
}
