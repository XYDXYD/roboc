using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class DestructionManager
	{
		private DestructionData _destructionInfo = new DestructionData();

		private FasterList<InstantiatedCube> _destroyedCubes = new FasterList<InstantiatedCube>(100);

		private FasterList<InstantiatedCube> _damagedCubes = new FasterList<InstantiatedCube>(100);

		private int _weaponDamage;

		[Inject]
		public PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public CubeHealingPropagator healingPropagator
		{
			private get;
			set;
		}

		[Inject]
		public PhysicsActivatorRunner physicsActivatorRunner
		{
			private get;
			set;
		}

		[Inject]
		public GraphicsUpdater graphicsUpdater
		{
			private get;
			set;
		}

		[Inject]
		public RigidbodyDataContainer rigidbodyDataContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public NetworkMachineManager machineManager
		{
			private get;
			set;
		}

		[Inject]
		public LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachineRootUpdater rootUpdater
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
		internal MachineRootContainer rootContainer
		{
			private get;
			set;
		}

		[Inject]
		internal LocalAIsContainer localAIs
		{
			private get;
			set;
		}

		public void PerformDestruction(List<HitCubeInfo> hitCubeInfo, int shootingPlayerId, int hitMachineId, int hitPlayerId, TargetType targetType, bool targetIsMe, bool playEffects, int weaponDamage, bool isReconnecting = false)
		{
			_weaponDamage = weaponDamage;
			_destroyedCubes.FastClear();
			_damagedCubes.FastClear();
			IMachineMap machineMap = machineManager.GetMachineMap(targetType, hitMachineId);
			if (livePlayersContainer.IsPlayerAlive(targetType, hitPlayerId) || isReconnecting)
			{
				for (int i = 0; i < hitCubeInfo.Count; i++)
				{
					HitCubeInfo hitCubeInfo2 = hitCubeInfo[i];
					UpdateCubeHealth(machineMap, ref hitCubeInfo2);
				}
				ApplyDestruction(targetIsMe, shootingPlayerId, hitPlayerId, hitMachineId, machineMap, targetType, playEffects);
			}
			else
			{
				Console.LogWarning("Received destruction for player who is not alive");
			}
		}

		private void UpdateCubeHealth(IMachineMap machineMap, ref HitCubeInfo hitCubeInfo)
		{
			if (machineMap == null)
			{
				return;
			}
			InstantiatedCube info = machineMap.GetCellAt(hitCubeInfo.gridLoc).info;
			if (!info.isDestroyed)
			{
				ApplyDamageToCube(ref hitCubeInfo, info);
				if (info.health == 0)
				{
					_destroyedCubes.Add(info);
				}
				else
				{
					_damagedCubes.Add(info);
				}
			}
		}

		private void ApplyDamageToCube(ref HitCubeInfo cubeHit, InstantiatedCube cube)
		{
			int health = cube.health;
			int damage = cubeHit.damage;
			if (cubeHit.destroyed)
			{
				damage = health;
			}
			else
			{
				int num = Mathf.Max(health - 1, 0);
				damage = Mathf.Min(damage, num);
			}
			cube.lastDamageApplied += damage;
			cube.health -= damage;
		}

		private void ApplyDestruction(bool targetIsMe, int shooterId, int hitPlayerId, int hitMachineId, IMachineMap machineMap, TargetType targetType, bool playEffects)
		{
			if (machineMap == null)
			{
				return;
			}
			int count = machineMap.GetRemainingCubes().Count;
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(targetType, hitMachineId);
			if (rigidBodyData == null)
			{
				return;
			}
			Transform transform = rigidBodyData.get_transform();
			if (transform == null)
			{
				Console.LogException(new Exception("ShootingException - DelayedCubesDestruction called with null RB"));
				return;
			}
			FasterList<InstantiatedCube> disconnectedCubes = rootUpdater.GetDisconnectedCubes(targetType, hitMachineId, _destroyedCubes);
			UpdateDestroyedCubes(disconnectedCubes);
			bool flag = false;
			if (targetType == TargetType.Player)
			{
				cpuDataManager.UpdateOnPlayerCubesDestroyed(shooterId, hitMachineId, _destroyedCubes, _damagedCubes);
				flag = (cpuDataManager.GetCurrentDisplayedHealthPercent(hitMachineId) <= 0f);
			}
			bool flag2 = targetIsMe || localAIs.IsAIHostedLocally(hitPlayerId);
			bool shooterIsLocal = playerTeamsContainer.IsMe(TargetType.Player, shooterId) || localAIs.IsAIHostedLocally(shooterId);
			_destructionInfo.Reset();
			_destructionInfo.isDestroyed = false;
			_destructionInfo.targetIsMe = targetIsMe;
			_destructionInfo.targetIsLocal = flag2;
			_destructionInfo.shooterId = shooterId;
			_destructionInfo.hitPlayerId = hitPlayerId;
			_destructionInfo.hitMachineId = hitMachineId;
			_destructionInfo.hitMachineMap = machineMap;
			_destructionInfo.hitRigidbody = rigidBodyData;
			_destructionInfo.shooterIsMe = playerTeamsContainer.IsMe(TargetType.Player, shooterId);
			_destructionInfo.shooterIsLocal = shooterIsLocal;
			_destructionInfo.destroyedCubes = _destroyedCubes;
			_destructionInfo.damagedCubes = _damagedCubes;
			_destructionInfo.targetType = targetType;
			_destructionInfo.weaponDamage = _weaponDamage;
			if (count - _destroyedCubes.get_Count() > 0)
			{
				DestroyCubes(_destructionInfo, playEffects);
			}
			if (flag && (targetType != 0 || flag2))
			{
				Console.Log("destroy machine " + hitMachineId);
				_destructionInfo.isDestroyed = true;
				DestroyMachine(_destructionInfo, playEffects: true);
			}
		}

		private void UpdateDestroyedCubes(FasterList<InstantiatedCube> separatedCubeInstances)
		{
			for (int i = 0; i < separatedCubeInstances.get_Count(); i++)
			{
				KillCube(separatedCubeInstances.get_Item(i));
				_destroyedCubes.Add(separatedCubeInstances.get_Item(i));
			}
			for (int num = _damagedCubes.get_Count() - 1; num >= 0; num--)
			{
				if (_damagedCubes.get_Item(num).health == 0)
				{
					_damagedCubes.UnorderedRemoveAt(num);
				}
			}
		}

		private void DestroyCubes(DestructionData data, bool playEffects)
		{
			if (_destroyedCubes.get_Count() > 0)
			{
				DeactivateCubes(_destroyedCubes, data.hitMachineMap);
			}
			physicsActivatorRunner.RunPhysicsActivatorOnDestroy(data);
			graphicsUpdater.RunGraphicsUpdaterOnDestroy(data);
			healingPropagator.UpdateCurableCubesOnDestroy(_destroyedCubes, _damagedCubes, data.targetType, data.hitMachineId);
			destructionReporter.PostCubeDestroyed(ref data);
			if (playEffects)
			{
				destructionReporter.PostCubeDestroyedEffects(ref data);
			}
			for (int i = 0; i < _damagedCubes.get_Count(); i++)
			{
				_damagedCubes.get_Item(i).lastDamageApplied = 0;
			}
			for (int j = 0; j < _destroyedCubes.get_Count(); j++)
			{
				_destroyedCubes.get_Item(j).lastDamageApplied = 0;
			}
		}

		private void DestroyMachine(DestructionData data, bool playEffects)
		{
			HashSet<InstantiatedCube> remainingCubes = data.hitMachineMap.GetRemainingCubes();
			if (remainingCubes.Count > 0)
			{
				_destroyedCubes.FastClear();
				_damagedCubes.FastClear();
				HashSet<InstantiatedCube>.Enumerator enumerator = remainingCubes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					InstantiatedCube current = enumerator.Current;
					KillCube(current);
					_destroyedCubes.Add(current);
				}
			}
			destructionReporter.PostCubeDestroyed(ref data);
			if (playEffects)
			{
				destructionReporter.PostCubeDestroyedEffects(ref data);
			}
		}

		private void DeactivateCubes(FasterList<InstantiatedCube> deadCubes, IMachineMap machineMap)
		{
			for (int i = 0; i < deadCubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = deadCubes.get_Item(i);
				instantiatedCube.health = 0;
				instantiatedCube.healingAmount = 0;
				machineMap.RemoveCube(instantiatedCube);
				if (!instantiatedCube.persistentCubeData.isGoReused)
				{
					MachineCell cellAt = machineMap.GetCellAt(instantiatedCube.gridPos);
					cellAt.gameObject.SetActive(false);
				}
				instantiatedCube.cubeNodeInstance.BreakLinks();
			}
		}

		public static void KillCube(InstantiatedCube cube)
		{
			cube.lastDamageApplied += cube.health;
			cube.health = 0;
		}

		internal void DestroyMachine(int playerId, int shooterId)
		{
			if (playerId == shooterId)
			{
				bool isMe = playerTeamsContainer.IsMe(TargetType.Player, playerId);
				int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
				destructionReporter.PlayerSelfDestructed(playerId, activeMachine, isMe);
				return;
			}
			int activeMachine2 = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
			Rigidbody rigidBodyData = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, activeMachine2);
			_destructionInfo.Reset();
			_destructionInfo.hitPlayerId = playerId;
			_destructionInfo.hitMachineId = activeMachine2;
			_destructionInfo.shooterId = shooterId;
			_destructionInfo.targetIsMe = false;
			_destructionInfo.shooterIsMe = playerTeamsContainer.IsMe(TargetType.Player, shooterId);
			_destructionInfo.isDestroyed = true;
			_destructionInfo.destroyedCubes = _destroyedCubes;
			_destructionInfo.damagedCubes = _damagedCubes;
			_destructionInfo.hitMachineMap = machineManager.GetMachineMap(TargetType.Player, activeMachine2);
			_destructionInfo.hitRigidbody = rigidBodyData;
			_destructionInfo.targetType = TargetType.Player;
			_destructionInfo.weaponDamage = _weaponDamage;
			DestroyMachine(_destructionInfo, playEffects: true);
		}
	}
}
