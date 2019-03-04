using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.DataStructures;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class HealingManager
	{
		private FasterList<InstantiatedCube> _healedCubes = new FasterList<InstantiatedCube>(100);

		private FasterList<InstantiatedCube> _respawnedCubes = new FasterList<InstantiatedCube>(100);

		private FasterList<InstantiatedCube> _itemsToRemove = new FasterList<InstantiatedCube>();

		private const float HEAL_RATE = 0.2f;

		private const float HEAL_DURATION = 1f;

		[Inject]
		internal HealingReporter healingReporter
		{
			private get;
			set;
		}

		[Inject]
		internal MachineClusterContainer machineClusterContainer
		{
			private get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
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
		internal NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		internal CubeHealingPropagator healingPropagator
		{
			private get;
			set;
		}

		[Inject]
		internal PhysicsActivatorRunner physicsActivatorRunner
		{
			private get;
			set;
		}

		[Inject]
		internal GraphicsUpdater graphicsUpdater
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal MachineCpuDataManager cpuDataManager
		{
			private get;
			set;
		}

		[Inject]
		internal LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		public bool PerformHealing(List<HitCubeInfo> hitCubeInfo, int shooterPlayerId, int hitMachineId, TargetType shooterTargetType, TargetType targetType, bool playEffects, bool isReconnecting = false)
		{
			bool result = false;
			_healedCubes.FastClear();
			_respawnedCubes.FastClear();
			IMachineMap machineMap = null;
			if (hitCubeInfo.Count > 0)
			{
				machineMap = machineManager.GetMachineMap(targetType, hitMachineId);
				int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(targetType, hitMachineId);
				if (livePlayersContainer.IsPlayerAlive(targetType, playerFromMachineId) || isReconnecting)
				{
					for (int i = 0; i < hitCubeInfo.Count; i++)
					{
						HitCubeInfo cubeHit = hitCubeInfo[i];
						UpdateHealth(machineMap, cubeHit);
					}
				}
				else
				{
					Console.LogWarning("Received healing for player who is not alive");
				}
			}
			if (ApplyHealing(shooterPlayerId, hitMachineId, machineMap, shooterTargetType, targetType, playEffects))
			{
				result = true;
			}
			return result;
		}

		public void FullyHealMachine(int hitMachineId, TargetType shooterTargetType, TargetType targetType, bool playEffects)
		{
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(targetType, hitMachineId);
			Console.Log("healing player " + playerFromMachineId);
			HashSet<InstantiatedCube> curableCubes = healingPropagator.GetCurableCubes(targetType, hitMachineId);
			while (curableCubes.Count > 0)
			{
				RespawnCubes(curableCubes, targetType, hitMachineId);
			}
		}

		public void FullyHealMachine(TargetType targetType, int machineId)
		{
			PrepareForRespawn(targetType, machineId);
			HashSet<InstantiatedCube> curableCubes = healingPropagator.GetCurableCubes(targetType, machineId);
			while (curableCubes.Count > 0)
			{
				RespawnCubes(curableCubes, targetType, machineId);
			}
		}

		private void UpdateHealth(IMachineMap machineMap, HitCubeInfo cubeHit)
		{
			if (machineMap == null)
			{
				return;
			}
			InstantiatedCube info = machineMap.GetCellAt(cubeHit.gridLoc).info;
			if (healingPropagator.ApplyHealing(info, cubeHit.damage))
			{
				if (info.lastHealingApplied == info.totalHealth)
				{
					_respawnedCubes.Add(info);
				}
				else
				{
					_healedCubes.Add(info);
				}
			}
		}

		private bool ApplyHealing(int shooterPlayerId, int hitMachineId, IMachineMap hitMachineMap, TargetType shooterTargetType, TargetType targetType, bool playEffects)
		{
			if (hitMachineMap == null)
			{
				return false;
			}
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitMachineId);
			if (rigidBodyData == null)
			{
				return false;
			}
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(targetType, hitMachineId);
			bool result = playerTeamsContainer.IsMe(targetType, playerFromMachineId);
			if (_respawnedCubes.get_Count() > 0)
			{
				RespawnCubes(_respawnedCubes, hitMachineMap, hitMachineId, targetType);
			}
			physicsActivatorRunner.RunPhysicsActivatorOnHeal(rigidBodyData, _respawnedCubes, hitMachineId, playerFromMachineId, targetType);
			graphicsUpdater.RunGraphicsUpdaterOnHeal(rigidBodyData.get_gameObject(), _respawnedCubes, _healedCubes);
			healingPropagator.UpdateCurableCubesOnHeal(_respawnedCubes, _healedCubes, targetType, hitMachineId);
			HealingData healingData = default(HealingData);
			healingData.shooterType = shooterTargetType;
			healingData.shooterPlayerId = shooterPlayerId;
			healingData.hitMachineId = hitMachineId;
			healingData.targetType = targetType;
			healingData.targetId = playerFromMachineId;
			healingData.respawnedCubes = _respawnedCubes;
			healingData.healedCubes = _healedCubes;
			HealingData data = healingData;
			healingReporter.PostCubeHealed(data);
			if (targetType == TargetType.Player)
			{
				cpuDataManager.UpdateOnPlayerCubesRespawned(shooterPlayerId, shooterTargetType, hitMachineId, _respawnedCubes, _healedCubes);
			}
			for (int i = 0; i < _healedCubes.get_Count(); i++)
			{
				_healedCubes.get_Item(i).lastHealingApplied = 0;
			}
			for (int j = 0; j < _respawnedCubes.get_Count(); j++)
			{
				_respawnedCubes.get_Item(j).lastHealingApplied = 0;
			}
			return result;
		}

		public void PrepareForRespawn(TargetType type, int machineId)
		{
			IMachineMap machineMap = machineManager.GetMachineMap(type, machineId);
			HashSet<InstantiatedCube> remainingCubes = machineMap.GetRemainingCubes();
			HashSet<InstantiatedCube>.Enumerator enumerator = remainingCubes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				current.health = current.totalHealth;
				current.lastDamageApplied = 0;
				current.lastHealingApplied = 0;
				current.healingAmount = 0;
				current.cubeNodeInstance.isDestroyed = false;
			}
		}

		internal void RespawnCubes(HashSet<InstantiatedCube> cubesToRespawn, TargetType type, int machineId)
		{
			IMachineMap machineMap = machineManager.GetMachineMap(type, machineId);
			_respawnedCubes.FastClear();
			_healedCubes.FastClear();
			HashSet<InstantiatedCube>.Enumerator enumerator = cubesToRespawn.GetEnumerator();
			while (enumerator.MoveNext())
			{
				InstantiatedCube current = enumerator.Current;
				if (current.health == 0)
				{
					if (!current.isDestroyed)
					{
						Console.LogError("trying to respawn non destroyed cube");
					}
					if (current.cubeNodeInstance.GetNeighbours().get_Count() > 0)
					{
						Console.LogError("trying to respawn cube with neighbour connections");
					}
					_respawnedCubes.Add(current);
				}
				else
				{
					_healedCubes.Add(current);
				}
				current.health = current.totalHealth;
			}
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(type, machineId);
			MachineCluster machineCluster = machineClusterContainer.GetMachineCluster(type, machineId);
			if (_respawnedCubes.get_Count() > 0)
			{
				RespawnCubes(_respawnedCubes, machineMap, machineId, type);
				machineCluster.AddLeaves(_respawnedCubes);
				PhysicsActivator.UpdatePhysics(rigidBodyData, null, _respawnedCubes);
			}
			graphicsUpdater.RunGraphicsUpdaterOnHeal(rigidBodyData.get_gameObject(), _respawnedCubes, _healedCubes);
			healingPropagator.UpdateCurableCubesOnHeal(_respawnedCubes, _healedCubes, type, machineId);
		}

		internal void RespawnCubes(FasterList<InstantiatedCube> respawnedCubes, IMachineMap machineMap, int hitMachineId, TargetType targetType)
		{
			for (int i = 0; i < respawnedCubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = respawnedCubes.get_Item(i);
				if (instantiatedCube.cubeNodeInstance.GetNeighbours().get_Count() > 0)
				{
					Console.LogError("respawning cube with " + instantiatedCube.cubeNodeInstance.GetNeighbours().get_Count() + " neighbours");
				}
				if (!instantiatedCube.cubeNodeInstance.isDestroyed)
				{
					Console.LogError("respawning cube that is not destroyed");
				}
				FasterList<CubeNodeInstance> originalNeighbours = instantiatedCube.cubeNodeInstance.GetOriginalNeighbours();
				int num = int.MaxValue;
				int num2 = -1;
				for (int j = 0; j < originalNeighbours.get_Count(); j++)
				{
					CubeNodeInstance cubeNodeInstance = originalNeighbours.get_Item(j);
					if (!cubeNodeInstance.isDestroyed)
					{
						instantiatedCube.cubeNodeInstance.ReAddLink(cubeNodeInstance);
						cubeNodeInstance.ReAddLink(instantiatedCube.cubeNodeInstance);
						if (cubeNodeInstance.numLinksToChair < num)
						{
							num = originalNeighbours.get_Item(j).numLinksToChair;
							num2 = j;
						}
					}
				}
				if (num2 != -1)
				{
					machineMap.AddCube(instantiatedCube);
					instantiatedCube.cubeNodeInstance.linkToChair = originalNeighbours.get_Item(num2);
					if (instantiatedCube.cubeNodeInstance.linkToChair.linkToChair == instantiatedCube.cubeNodeInstance)
					{
						Console.LogError("link to chair is cyclic");
					}
					instantiatedCube.cubeNodeInstance.isDestroyed = false;
					GameObject cubeAt = machineMap.GetCubeAt(instantiatedCube.gridPos);
					if (cubeAt != null)
					{
						cubeAt.SetActive(true);
					}
				}
				else
				{
					_itemsToRemove.Add(instantiatedCube);
					instantiatedCube.health = 0;
					instantiatedCube.lastHealingApplied = 0;
				}
			}
			for (int num3 = _itemsToRemove.get_Count() - 1; num3 >= 0; num3--)
			{
				respawnedCubes.UnorderedRemove(_itemsToRemove.get_Item(num3));
			}
			if (_itemsToRemove.get_Count() > 0)
			{
				healingPropagator.UpdateCurableCubesOnDestroy(_itemsToRemove, null, targetType, hitMachineId);
			}
			_itemsToRemove.FastClear();
		}
	}
}
