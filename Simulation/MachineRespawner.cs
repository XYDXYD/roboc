using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Ticker.Legacy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class MachineRespawner : IPhysicallyTickable, ITickableBase
	{
		private FasterList<Action> _setupOnPhysicsUpdate = new FasterList<Action>();

		private MachineColliderCollectionData _machineColliderCollectionData = new MachineColliderCollectionData();

		[Inject]
		internal CubeHealingPropagator healingPropagator
		{
			private get;
			set;
		}

		[Inject]
		internal HealingManager healingManager
		{
			private get;
			set;
		}

		[Inject]
		internal MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher machineDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal RemoteClientHistoryClient remoteClientHistory
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

		[Inject]
		internal PlayerTeamsContainer playerTeams
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer playerNames
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachines
		{
			private get;
			set;
		}

		[Inject]
		internal ConnectedPlayersContainer connectedPlayers
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
		internal MachineClusterContainer machineClusterContainer
		{
			private get;
			set;
		}

		[Inject]
		internal GameEndedObserver gameEndedObserver
		{
			get;
			set;
		}

		[Inject]
		internal MachineColliderCollectionObservable machineColliderCollectionObservable
		{
			private get;
			set;
		}

		internal void StartRespawn(SpawnPointDependency dependency)
		{
			TaskRunner.get_Instance().Run(Respawn(dependency));
		}

		private IEnumerator Respawn(SpawnPointDependency dependency)
		{
			int playerId = dependency.owner;
			int machineId = playerMachines.GetActiveMachine(TargetType.Player, playerId);
			healingManager.PrepareForRespawn(TargetType.Player, machineId);
			HashSet<InstantiatedCube> curableCubes = healingPropagator.GetCurableCubes(TargetType.Player, machineId);
			Rigidbody rb = rigidbodyDataContainer.GetRigidBodyData(TargetType.Player, machineId);
			while (curableCubes.Count > 0)
			{
				healingManager.RespawnCubes(curableCubes, TargetType.Player, machineId);
				yield return null;
			}
			bool isMe = playerTeams.IsMe(TargetType.Player, playerId);
			MachineCluster machineCluster = machineClusterContainer.GetMachineCluster(TargetType.Player, machineId);
			MicrobotCollisionSphere microbotCollisionSphere = machineClusterContainer.GetMicrobotCollisionSphere(TargetType.Player, machineId);
			_machineColliderCollectionData.ResetData(machineId);
			machineCluster.CreateGameObjectStructure(rb.get_transform(), _machineColliderCollectionData.NewColliders, _machineColliderCollectionData.RemovedColliders);
			if (connectedPlayers.IsPlayerConnected(playerId))
			{
				PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(machineId);
				int teamId = playerTeams.GetPlayerTeam(TargetType.Player, playerId);
				Action action = delegate
				{
					//IL_005d: Unknown result type (might be due to invalid IL or missing references)
					//IL_006d: Unknown result type (might be due to invalid IL or missing references)
					//IL_007d: Unknown result type (might be due to invalid IL or missing references)
					//IL_008d: Unknown result type (might be due to invalid IL or missing references)
					if (connectedPlayers.IsPlayerConnected(playerId))
					{
						preloadedMachine.machineBoard.SetActive(true);
						SetSpawnPosition(rb, preloadedMachine, dependency.rotation, dependency.position, dependency.velocity, dependency.angularVelocity);
						if (!isMe)
						{
							remoteClientHistory.AddMachine(playerId, preloadedMachine.machineId, preloadedMachine.rbData);
						}
						livePlayersContainer.MarkAsLive(TargetType.Player, playerId);
						string playerName = playerNames.GetPlayerName(playerId);
						string displayName = playerNames.GetDisplayName(playerId);
						SpawnInParametersPlayer spawnInParameters = new SpawnInParametersPlayer(playerId, machineId, playerName, teamId, isMe, playerTeams.IsMyTeam(teamId), preloadedMachine, _isAImachine: false, isMe);
						machineDispatcher.PlayerRespawnedIn(spawnInParameters);
					}
				};
				microbotCollisionSphere.TryActivateSphereCollider(rb.get_transform(), machineCluster, preloadedMachine.machineInfo.MachineCenter, _machineColliderCollectionData.NewColliders);
				machineColliderCollectionObservable.Dispatch(ref _machineColliderCollectionData);
				_setupOnPhysicsUpdate.Add(action);
			}
		}

		private void SetSpawnPosition(Rigidbody rb, PreloadedMachine preloadedMachine, Quaternion rot, Vector3 pos, Vector3 vel, Vector3 angVel)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			MachineInfo machineInfo = preloadedMachine.machineInfo;
			MachineSpawnUtility.AdjustSpawnPosition(machineInfo, rb, ref pos, ref rot);
			rb.set_position(pos);
			rb.set_rotation(rot);
			rb.set_velocity(vel);
			rb.AddTorque(angVel, 2);
		}

		public void PhysicsTick(float deltaSec)
		{
			while (_setupOnPhysicsUpdate.get_Count() > 0)
			{
				_setupOnPhysicsUpdate.get_Item(0)();
				_setupOnPhysicsUpdate.UnorderedRemoveAt(0);
			}
		}
	}
}
