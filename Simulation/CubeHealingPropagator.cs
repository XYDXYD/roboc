using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class CubeHealingPropagator : IInitialize, IWaitForFrameworkDestruction
	{
		private HealingGraphTraverser _healingGraphTraverser = new HealingGraphTraverser();

		private Dictionary<int, HashSet<InstantiatedCube>> _curableCubesPerMachine = new Dictionary<int, HashSet<InstantiatedCube>>();

		[Inject]
		internal MachineSpawnDispatcher machineSpawner
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			machineSpawner.OnPlayerRegistered += HandleOnPlayerRegistered;
			machineSpawner.OnPlayerSpawnedOut += HandleOnPlayerSpawnedOut;
		}

		private void HandleOnPlayerSpawnedOut(SpawnOutParameters paramaters)
		{
			Console.Log("PLAYER SPAWNED OUT:" + paramaters.playerId.ToString());
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineSpawner.OnPlayerRegistered -= HandleOnPlayerRegistered;
			machineSpawner.OnPlayerSpawnedOut -= HandleOnPlayerSpawnedOut;
		}

		private void HandleOnPlayerRegistered(SpawnInParametersPlayer spawnInParameters)
		{
			int machineId = spawnInParameters.preloadedMachine.machineId;
			_curableCubesPerMachine[machineId] = new HashSet<InstantiatedCube>();
		}

		public bool ApplyHealing(InstantiatedCube cube, int healToThisCube)
		{
			int totalHealth = cube.totalHealth;
			if (cube.health == 0)
			{
				cube.healingAmount += healToThisCube;
				if (cube.healingAmount >= totalHealth)
				{
					cube.lastHealingApplied = totalHealth;
					cube.health = totalHealth;
					cube.healingAmount = 0;
					return true;
				}
				cube.lastHealingApplied = 0;
				return false;
			}
			cube.lastHealingApplied = Mathf.Min(healToThisCube, totalHealth - cube.health);
			cube.health += cube.lastHealingApplied;
			cube.healingAmount = 0;
			return cube.lastHealingApplied > 0;
		}

		internal void ComputeProposedHeal(int hitMachineId, InstantiatedCube cube, int heal, ref Dictionary<InstantiatedCube, int> proposedResult)
		{
			int healDealt = 0;
			SelectCubesToHeal(cube, hitMachineId, heal, damageAllCubeTypes: true, ref healDealt, ref proposedResult);
		}

		internal void ComputeProposedHeal(TargetType type, int hitMachineId, int heal, ref Dictionary<InstantiatedCube, int> proposedResult)
		{
			int healDealt = 0;
			SelectCubesToHeal(type, hitMachineId, heal, ref healDealt, ref proposedResult);
		}

		private void SelectCubesToHeal(InstantiatedCube cube, int hitMachineId, int heal, bool damageAllCubeTypes, ref int healDealt, ref Dictionary<InstantiatedCube, int> healedCubes)
		{
			if (cube.health != cube.totalHealth)
			{
				AddHealedCube(cube, heal, ref healDealt, ref healedCubes);
			}
			FasterList<CubeNodeInstance> originalNeighbours = cube.cubeNodeInstance.GetOriginalNeighbours();
			for (int i = 0; i < originalNeighbours.get_Count(); i++)
			{
				if (healDealt < heal)
				{
					InstantiatedCube instantiatedCube = originalNeighbours.get_Item(i).instantiatedCube;
					int health = instantiatedCube.health;
					int totalHealth = instantiatedCube.totalHealth;
					if (health != totalHealth && health + instantiatedCube.pendingHealing < totalHealth)
					{
						AddHealedCube(instantiatedCube, heal, ref healDealt, ref healedCubes);
					}
					continue;
				}
				return;
			}
			if (!_curableCubesPerMachine.TryGetValue(hitMachineId, out HashSet<InstantiatedCube> value))
			{
				return;
			}
			HashSet<InstantiatedCube>.Enumerator enumerator = value.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				int health2 = current.health;
				int totalHealth2 = current.totalHealth;
				if ((!healedCubes.ContainsKey(current) || healedCubes[current] + health2 != totalHealth2) && health2 + current.pendingHealing < totalHealth2)
				{
					if (healDealt >= heal)
					{
						break;
					}
					AddHealedCube(current, heal, ref healDealt, ref healedCubes);
				}
			}
		}

		private void SelectCubesToHeal(TargetType type, int hitMachineId, int heal, ref int healDealt, ref Dictionary<InstantiatedCube, int> healedCubes)
		{
			if (!_curableCubesPerMachine.TryGetValue(hitMachineId, out HashSet<InstantiatedCube> value))
			{
				return;
			}
			HashSet<InstantiatedCube>.Enumerator enumerator = value.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				if (!healedCubes.ContainsKey(current) || healedCubes[current] + current.health != current.totalHealth)
				{
					if (healDealt >= heal)
					{
						break;
					}
					AddHealedCube(current, heal, ref healDealt, ref healedCubes);
				}
			}
		}

		internal void ComputeProposedAutoHeal(int hitMachineId, int totalHealingRequired, ref List<HitCubeInfo> healedCubes)
		{
			_healingGraphTraverser.SelectCubesToHeal(totalHealingRequired, healedCubes, _curableCubesPerMachine[hitMachineId]);
		}

		internal void GenerateHealingGroupHitInfo(Dictionary<InstantiatedCube, int> healedCubes, List<HitCubeInfo> destroyedCubes)
		{
			Dictionary<InstantiatedCube, int>.Enumerator enumerator = healedCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<InstantiatedCube, int> current = enumerator.Current;
				HitCubeInfo item = default(HitCubeInfo);
				item.damage = current.Value;
				item.gridLoc = current.Key.gridPos;
				destroyedCubes.Add(item);
			}
		}

		private void AddHealedCube(InstantiatedCube cubeInstance, int heal, ref int healAlreadyDealt, ref Dictionary<InstantiatedCube, int> healedCubes)
		{
			int num = (cubeInstance.health != 0) ? cubeInstance.health : cubeInstance.healingAmount;
			int num2 = healedCubes.ContainsKey(cubeInstance) ? healedCubes[cubeInstance] : 0;
			int num3 = cubeInstance.totalHealth - num2 - cubeInstance.pendingHealing - num;
			if (num3 < 0)
			{
				num3 = 0;
			}
			int num4 = Mathf.Min(num3, heal - healAlreadyDealt);
			if (num4 > 0)
			{
				healAlreadyDealt += num4;
				healedCubes[cubeInstance] = num2 + num4;
			}
		}

		public void UpdateCurableCubesOnDestroy(FasterList<InstantiatedCube> destroyed, FasterList<InstantiatedCube> damaged, TargetType type, int machineId)
		{
			if (type != 0)
			{
				return;
			}
			HashSet<InstantiatedCube> hashSet = _curableCubesPerMachine[machineId];
			if (damaged != null)
			{
				for (int i = 0; i < damaged.get_Count(); i++)
				{
					InstantiatedCube instantiatedCube = damaged.get_Item(i);
					if (!hashSet.Contains(instantiatedCube) && instantiatedCube.lastDamageApplied > 0)
					{
						hashSet.Add(instantiatedCube);
					}
				}
			}
			for (int j = 0; j < destroyed.get_Count(); j++)
			{
				InstantiatedCube instantiatedCube2 = destroyed.get_Item(j);
				if (IsConnected(instantiatedCube2))
				{
					hashSet.Add(instantiatedCube2);
				}
				else
				{
					hashSet.Remove(instantiatedCube2);
				}
				FasterList<CubeNodeInstance> originalNeighbours = instantiatedCube2.cubeNodeInstance.GetOriginalNeighbours();
				for (int k = 0; k < originalNeighbours.get_Count(); k++)
				{
					InstantiatedCube instantiatedCube3 = originalNeighbours.get_Item(k).instantiatedCube;
					if (!IsConnected(instantiatedCube3))
					{
						hashSet.Remove(instantiatedCube3);
					}
				}
			}
		}

		public void UpdateCurableCubesOnHeal(FasterList<InstantiatedCube> respawn, FasterList<InstantiatedCube> healed, TargetType type, int machineId)
		{
			if (type != 0)
			{
				return;
			}
			if (!_curableCubesPerMachine.TryGetValue(machineId, out HashSet<InstantiatedCube> value))
			{
				Console.LogError("UpdateCurableCubesOnHeal received while machine " + machineId + " isn't registered");
				return;
			}
			for (int i = 0; i < healed.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = healed.get_Item(i);
				if (instantiatedCube.health == instantiatedCube.totalHealth)
				{
					value.Remove(instantiatedCube);
				}
			}
			for (int j = 0; j < respawn.get_Count(); j++)
			{
				InstantiatedCube instantiatedCube2 = respawn.get_Item(j);
				value.Remove(instantiatedCube2);
				FasterList<CubeNodeInstance> originalNeighbours = instantiatedCube2.cubeNodeInstance.GetOriginalNeighbours();
				for (int k = 0; k < originalNeighbours.get_Count(); k++)
				{
					InstantiatedCube instantiatedCube3 = originalNeighbours.get_Item(k).instantiatedCube;
					if (!value.Contains(instantiatedCube3) && instantiatedCube3.isDestroyed)
					{
						value.Add(instantiatedCube3);
					}
				}
			}
		}

		public bool PlayerCanBeHealed(TargetType type, int hitMachineId)
		{
			if (type == TargetType.Player && _curableCubesPerMachine.TryGetValue(hitMachineId, out HashSet<InstantiatedCube> value))
			{
				return value.Count > 0;
			}
			return false;
		}

		public HashSet<InstantiatedCube> GetCurableCubes(TargetType type, int hitMachineId)
		{
			if (_curableCubesPerMachine.TryGetValue(hitMachineId, out HashSet<InstantiatedCube> value))
			{
				return value;
			}
			return null;
		}

		private bool IsConnected(InstantiatedCube cube)
		{
			FasterList<CubeNodeInstance> originalNeighbours = cube.cubeNodeInstance.GetOriginalNeighbours();
			for (int i = 0; i < originalNeighbours.get_Count(); i++)
			{
				if (!originalNeighbours.get_Item(i).instantiatedCube.isDestroyed)
				{
					return true;
				}
			}
			return false;
		}
	}
}
