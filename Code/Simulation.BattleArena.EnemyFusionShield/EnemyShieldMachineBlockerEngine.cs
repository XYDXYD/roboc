using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Simulation.BattleArena.EnemyFusionShield
{
	internal class EnemyShieldMachineBlockerEngine : IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction, IEngine
	{
		[Inject]
		internal GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColliderCollectionObserver machineColliderCollectionObserver
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColliderIgnoreObserver machineColliderIgnoreObserver
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			gameStartDispatcher.Register(HandleGameStart);
			spawnDispatcher.OnPlayerRespawnedIn += HandlePlayerRespawned;
			machineColliderCollectionObserver.AddAction(new ObserverAction<MachineColliderCollectionData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			machineColliderIgnoreObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(HandleGameStart);
			spawnDispatcher.OnPlayerRespawnedIn -= HandlePlayerRespawned;
			machineColliderCollectionObserver.RemoveAction(new ObserverAction<MachineColliderCollectionData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			machineColliderIgnoreObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleGameStart()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<InsideFusionShieldEntityView> val = entityViewsDB.QueryEntityViews<InsideFusionShieldEntityView>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				int machineId = val.get_Item(i).machineOwnerComponent.ownerMachineId;
				ChangeMachineColliderIgnores(ref machineId);
			}
		}

		private void HandlePlayerRespawned(SpawnInParametersPlayer spawnInParametersPlayer)
		{
			int machineId = spawnInParametersPlayer.machineId;
			ChangeMachineColliderIgnores(ref machineId);
		}

		private void HandleMachineColliderCollectionChanged(ref MachineColliderCollectionData machineColliderCollectionData)
		{
			InsideFusionShieldEntityView playerMachine = default(InsideFusionShieldEntityView);
			if (entityViewsDB.TryQueryEntityView<InsideFusionShieldEntityView>(machineColliderCollectionData.MachineId, ref playerMachine))
			{
				UpdateFriendlyShieldCollision(playerMachine, machineColliderCollectionData.NewColliders, machineColliderCollectionData.RemovedColliders);
				UpdateEnemyShieldCollision(playerMachine, machineColliderCollectionData.NewColliders, machineColliderCollectionData.RemovedColliders);
			}
		}

		private void ChangeMachineColliderIgnores(ref int machineId)
		{
			InsideFusionShieldEntityView playerMachine = default(InsideFusionShieldEntityView);
			if (entityViewsDB.TryQueryEntityView<InsideFusionShieldEntityView>(machineId, ref playerMachine))
			{
				MachineCollidersEntityView machineCollidersEntityView = entityViewsDB.QueryEntityView<MachineCollidersEntityView>(machineId);
				UpdateFriendlyShieldCollision(playerMachine, machineCollidersEntityView.machineCollidersComponent.colliders, null);
				UpdateEnemyShieldCollision(playerMachine, machineCollidersEntityView.machineCollidersComponent.colliders, null);
			}
		}

		private void UpdateFriendlyShieldCollision(InsideFusionShieldEntityView playerMachine, FasterList<Collider> newColliders, FasterList<Collider> removedColliders)
		{
			FusionShieldEntityView fusionShieldEntityView = default(FusionShieldEntityView);
			if (entityViewsDB.TryQueryEntityView<FusionShieldEntityView>(playerMachine.ownerTeamComponent.ownerTeamId, ref fusionShieldEntityView))
			{
				MeshCollider shieldMeshCollider = fusionShieldEntityView.fusionShieldColliderComponent.shieldMeshCollider;
				ChangeCollisionIgnore(newColliders, shieldMeshCollider, shouldIgnore: true);
				ChangeCollisionIgnore(removedColliders, shieldMeshCollider, shouldIgnore: false);
			}
		}

		private void UpdateEnemyShieldCollision(InsideFusionShieldEntityView playerMachine, FasterList<Collider> newColliders, FasterList<Collider> removedColliders)
		{
			FusionShieldEntityView fusionShieldEntityView = default(FusionShieldEntityView);
			if (playerMachine.insideFusionShieldComponent.teamId != -1 && playerMachine.insideFusionShieldComponent.teamId != playerMachine.ownerTeamComponent.ownerTeamId && entityViewsDB.TryQueryEntityView<FusionShieldEntityView>(playerMachine.insideFusionShieldComponent.teamId, ref fusionShieldEntityView))
			{
				MeshCollider shieldMeshCollider = fusionShieldEntityView.fusionShieldColliderComponent.shieldMeshCollider;
				bool shouldIgnore = playerMachine.insideFusionShieldComponent.isInsideShield && playerMachine.insideFusionShieldComponent.isEncapsulated;
				ChangeCollisionIgnore(newColliders, shieldMeshCollider, shouldIgnore);
				ChangeCollisionIgnore(removedColliders, shieldMeshCollider, shouldIgnore: false);
			}
		}

		private void ChangeCollisionIgnore(FasterList<Collider> machineColliders, Collider fusionShieldCollider, bool shouldIgnore)
		{
			if (machineColliders != null)
			{
				for (int i = 0; i < machineColliders.get_Count(); i++)
				{
					Physics.IgnoreCollision(machineColliders.get_Item(i), fusionShieldCollider, shouldIgnore);
				}
			}
		}
	}
}
