using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class PhysicsActivatorRunner : ILateTickable, ITickableBase
	{
		private struct LateUpdateInfo
		{
			public readonly int machineId;

			public readonly int hitPlayerId;

			public readonly MachineCluster machineCluster;

			public readonly MicrobotCollisionSphere microbotCollision;

			public readonly Rigidbody rigidbody;

			public readonly TargetType targetType;

			public LateUpdateInfo(int machineId, Rigidbody rb, int hitPlayerId, MachineCluster m, MicrobotCollisionSphere c, TargetType targetType)
			{
				this.machineId = machineId;
				rigidbody = rb;
				this.hitPlayerId = hitPlayerId;
				machineCluster = m;
				microbotCollision = c;
				this.targetType = targetType;
			}
		}

		private HashSet<int> _currentLateUpdaters = new HashSet<int>();

		private MachineColliderCollectionData _machineColliderCollectionData = new MachineColliderCollectionData();

		private Stack<LateUpdateInfo> _lateUpdateInfo = new Stack<LateUpdateInfo>();

		[Inject]
		public MachineClusterContainer machineClusterContainer
		{
			private get;
			set;
		}

		[Inject]
		public MachineColliderCollectionObservable machineColliderCollectionObservable
		{
			private get;
			set;
		}

		public void LateTick(float deltaTime)
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			while (_lateUpdateInfo.Count > 0)
			{
				LateUpdateInfo lateUpdateInfo = _lateUpdateInfo.Pop();
				_currentLateUpdaters.Remove(lateUpdateInfo.machineId);
				Rigidbody rigidbody = lateUpdateInfo.rigidbody;
				if (rigidbody != null && rigidbody.get_gameObject().get_activeInHierarchy())
				{
					_machineColliderCollectionData.ResetData(lateUpdateInfo.machineId);
					lateUpdateInfo.machineCluster.CreateGameObjectStructure(rigidbody.get_transform(), _machineColliderCollectionData.NewColliders, _machineColliderCollectionData.RemovedColliders);
					if (lateUpdateInfo.targetType == TargetType.Player)
					{
						lateUpdateInfo.microbotCollision.TryActivateSphereCollider(rigidbody.get_transform(), lateUpdateInfo.machineCluster, rigidbody.get_centerOfMass(), _machineColliderCollectionData.NewColliders);
					}
					machineColliderCollectionObservable.Dispatch(ref _machineColliderCollectionData);
				}
			}
		}

		public void RunPhysicsActivatorOnDestroy(DestructionData data)
		{
			if (data.destroyedCubes.get_Count() > 0)
			{
				MachineCluster machineCluster = machineClusterContainer.GetMachineCluster(data.targetType, data.hitMachineId);
				machineCluster.RemoveLeaves(data.destroyedCubes);
				PhysicsActivator.UpdatePhysics(data.hitRigidbody, data.destroyedCubes, null);
				MicrobotCollisionSphere c = null;
				if (data.targetType == TargetType.Player)
				{
					c = machineClusterContainer.GetMicrobotCollisionSphere(data.targetType, data.hitMachineId);
				}
				if (!_currentLateUpdaters.Contains(data.hitMachineId))
				{
					_currentLateUpdaters.Add(data.hitMachineId);
					LateUpdateInfo item = new LateUpdateInfo(data.hitMachineId, data.hitRigidbody, data.hitPlayerId, machineCluster, c, data.targetType);
					_lateUpdateInfo.Push(item);
				}
			}
		}

		public void RunPhysicsActivatorOnHeal(Rigidbody rb, FasterList<InstantiatedCube> respawnedCubes, int hitMachineId, int hitPlayerId, TargetType targetType)
		{
			if (respawnedCubes.get_Count() > 0)
			{
				MachineCluster machineCluster = machineClusterContainer.GetMachineCluster(targetType, hitMachineId);
				machineCluster.AddLeaves(respawnedCubes);
				PhysicsActivator.UpdatePhysics(rb, null, respawnedCubes);
				MicrobotCollisionSphere c = null;
				if (targetType == TargetType.Player)
				{
					c = machineClusterContainer.GetMicrobotCollisionSphere(targetType, hitMachineId);
				}
				if (!_currentLateUpdaters.Contains(hitMachineId))
				{
					_currentLateUpdaters.Add(hitMachineId);
					LateUpdateInfo item = new LateUpdateInfo(hitMachineId, rb, hitPlayerId, machineCluster, c, targetType);
					_lateUpdateInfo.Push(item);
				}
			}
		}
	}
}
