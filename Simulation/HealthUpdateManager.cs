using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class HealthUpdateManager : IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		internal DestructionReporter damageReporter
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal HealingReporter healingReporter
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			damageReporter.OnPlayerDamageApplied += HandleOnPlayerDamageApplied;
			healingReporter.OnPlayerCubesHealed += HandleOnPlayerCubesHealed;
			healingReporter.OnPlayerCubesRespawned += HandleOnPlayerCubesHealed;
			EnableShaderKeywords();
		}

		private void EnableShaderKeywords()
		{
			Shader.DisableKeyword("RED_ON");
			Shader.EnableKeyword("RED_OFF");
		}

		private void HandleOnPlayerCubesHealed(int shooterId, int machineId, FasterList<InstantiatedCube> cubeToUpdate, TargetType shooterTargetType)
		{
			UpdateVisualCubeHealth(machineId, cubeToUpdate);
		}

		private void UpdateVisualCubeHealth(int machineId, FasterList<InstantiatedCube> cubeToUpdate)
		{
			IMachineMap machineMap = machineManager.GetMachineMap(TargetType.Player, machineId);
			UpdateVisualCubeHealth(machineMap, cubeToUpdate);
		}

		private void UpdateVisualCubeHealth(IMachineMap map, FasterList<InstantiatedCube> cubesToUpdate)
		{
			for (int i = 0; i < cubesToUpdate.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = cubesToUpdate.get_Item(i);
				if (!instantiatedCube.persistentCubeData.isBatchable)
				{
					GameObject cubeAt = map.GetCubeAt(instantiatedCube.gridPos);
					float num = (float)instantiatedCube.health / (float)instantiatedCube.totalHealth;
					if (num < 1f)
					{
						num = Math.Min(num, 0.8f);
					}
					cubeAt.GetComponent<MachinePartColorUpdater>().SetHealth(num);
				}
			}
		}

		private void HandleOnPlayerDamageApplied(DestructionData data)
		{
			UpdateVisualCubeHealth(data.hitMachineMap, data.damagedCubes);
		}

		public void OnFrameworkDestroyed()
		{
			damageReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
			healingReporter.OnPlayerCubesHealed -= HandleOnPlayerCubesHealed;
			healingReporter.OnPlayerCubesRespawned -= HandleOnPlayerCubesHealed;
		}
	}
}
