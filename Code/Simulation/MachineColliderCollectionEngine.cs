using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class MachineColliderCollectionEngine : SingleEntityViewEngine<MachineCollidersEntityView>, IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction, IEngine
	{
		private FasterList<Collider> _newMachineColliders = new FasterList<Collider>();

		[Inject]
		internal MachinePreloader machinePreloader
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
		internal MachineColliderCollectionObserver machineColliderCollectionObserver
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
			machineColliderCollectionObserver.AddAction(new ObserverAction<MachineColliderCollectionData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineColliderCollectionObserver.RemoveAction(new ObserverAction<MachineColliderCollectionData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(MachineCollidersEntityView entityView)
		{
			CollectCollidersForMachine(entityView);
		}

		protected override void Remove(MachineCollidersEntityView entityView)
		{
		}

		private void CollectCollidersForMachine(MachineCollidersEntityView playerMachine)
		{
			playerMachine.machineCollidersComponent.colliders = new FasterList<Collider>();
			CollectCollidersFromCubeTransforms(playerMachine);
			CollectCollidersFromMachineCluster(playerMachine);
			CollectCollidersFromMicrobotCollisionSphere(playerMachine);
		}

		private void CollectCollidersFromCubeTransforms(MachineCollidersEntityView playerMachine)
		{
			int ownerMachineId = playerMachine.machineOwnerComponent.ownerMachineId;
			PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(ownerMachineId);
			List<Collider> list = new List<Collider>();
			for (int i = 0; i < preloadedMachine.allCubeTransforms.get_Count(); i++)
			{
				preloadedMachine.allCubeTransforms.get_Item(i).GetComponentsInChildren<Collider>(true, list);
				playerMachine.machineCollidersComponent.colliders.AddRange((ICollection<Collider>)list);
			}
		}

		private void CollectCollidersFromMachineCluster(MachineCollidersEntityView playerMachine)
		{
			int ownerMachineId = playerMachine.machineOwnerComponent.ownerMachineId;
			FasterList<NodeBoxCollider> colliders = machineClusterContainer.GetMachineCluster(TargetType.Player, ownerMachineId).GetColliders();
			for (int i = 0; i < colliders.get_Count(); i++)
			{
				FasterList<Collider> colliders2 = playerMachine.machineCollidersComponent.colliders;
				NodeBoxCollider nodeBoxCollider = colliders.get_Item(i);
				colliders2.Add(nodeBoxCollider.collider);
			}
		}

		private void CollectCollidersFromMicrobotCollisionSphere(MachineCollidersEntityView playerMachine)
		{
			int ownerMachineId = playerMachine.machineOwnerComponent.ownerMachineId;
			MicrobotCollisionSphere microbotCollisionSphere = machineClusterContainer.GetMicrobotCollisionSphere(TargetType.Player, ownerMachineId);
			SphereCollider sphereCollider = microbotCollisionSphere.GetSphereCollider();
			playerMachine.machineCollidersComponent.colliders.Add(sphereCollider);
			FasterList<Collider> nonClusteredColliders = microbotCollisionSphere.GetNonClusteredColliders();
			playerMachine.machineCollidersComponent.colliders.AddRange(nonClusteredColliders);
		}

		private void HandleMachineColliderCollectionChanged(ref MachineColliderCollectionData machineColliderCollectionData)
		{
			MachineCollidersEntityView playerMachine = default(MachineCollidersEntityView);
			if (entityViewsDB.TryQueryEntityView<MachineCollidersEntityView>(machineColliderCollectionData.MachineId, ref playerMachine))
			{
				RemoveCollidersFromCollection(playerMachine, machineColliderCollectionData.RemovedColliders);
				AddNewCollidersToCollection(playerMachine, machineColliderCollectionData.NewColliders);
			}
		}

		private void RemoveCollidersFromCollection(MachineCollidersEntityView playerMachine, FasterList<Collider> removedColliders)
		{
			FasterList<Collider> colliders = playerMachine.machineCollidersComponent.colliders;
			for (int i = 0; i < removedColliders.get_Count(); i++)
			{
				Collider val = removedColliders.get_Item(i);
				for (int num = colliders.get_Count() - 1; num >= 0; num--)
				{
					Collider val2 = colliders.get_Item(num);
					if (val2 == val)
					{
						colliders.UnorderedRemove(val2);
					}
				}
			}
		}

		private void AddNewCollidersToCollection(MachineCollidersEntityView playerMachine, FasterList<Collider> newColliders)
		{
			_newMachineColliders.Clear();
			FasterList<Collider> colliders = playerMachine.machineCollidersComponent.colliders;
			for (int i = 0; i < newColliders.get_Count(); i++)
			{
				Collider val = newColliders.get_Item(i);
				if (!colliders.Contains(val))
				{
					_newMachineColliders.Add(val);
				}
			}
			colliders.AddRange(_newMachineColliders);
		}
	}
}
