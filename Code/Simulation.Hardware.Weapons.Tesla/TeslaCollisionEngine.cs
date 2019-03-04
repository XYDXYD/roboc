using Svelto.ECS;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal sealed class TeslaCollisionEngine : SingleEntityViewEngine<TeslaRamCollisionNode>, IQueryingEntityViewEngine, IEngine
	{
		[Inject]
		internal MachineRootContainer machineRootContainer
		{
			get;
			private set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			get;
			private set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		public Type[] AcceptedNodes()
		{
			return new Type[1]
			{
				typeof(TeslaRamCollisionNode)
			};
		}

		protected override void Add(TeslaRamCollisionNode node)
		{
			if (node.entitySourceComponent.isLocal)
			{
				ICollisionComponent collisionComponent = node.collisionComponent;
				collisionComponent.onTriggerEnter.NotifyOnValueSet((Action<int, Collider>)OnTriggerEnter);
				collisionComponent.onTriggerExit.NotifyOnValueSet((Action<int, Collider>)OnTriggerExit);
			}
		}

		protected override void Remove(TeslaRamCollisionNode node)
		{
			IHardwareOwnerComponent weaponOwnerComponent = node.weaponOwnerComponent;
			if (weaponOwnerComponent.ownedByMe || weaponOwnerComponent.ownedByAi)
			{
				ICollisionComponent collisionComponent = node.collisionComponent;
				collisionComponent.onTriggerEnter.StopNotify((Action<int, Collider>)OnTriggerEnter);
				collisionComponent.onTriggerExit.StopNotify((Action<int, Collider>)OnTriggerExit);
			}
		}

		private void OnTriggerEnter(int weaponId, Collider c)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			TeslaRamCollisionNode teslaRamCollisionNode = default(TeslaRamCollisionNode);
			if (entityViewsDB.TryQueryEntityView<TeslaRamCollisionNode>(weaponId, ref teslaRamCollisionNode) && teslaRamCollisionNode.weaponActiveComponent.active && c.get_gameObject().get_layer() != GameLayers.AI_COLLISION)
			{
				Vector3 position = teslaRamCollisionNode.hitPositionComponent.position;
				CollisionEnter(teslaRamCollisionNode, c, position);
			}
		}

		private void OnTriggerExit(int weaponId, Collider c)
		{
			TeslaRamCollisionNode teslaRamCollisionNode = default(TeslaRamCollisionNode);
			if (entityViewsDB.TryQueryEntityView<TeslaRamCollisionNode>(weaponId, ref teslaRamCollisionNode))
			{
				CollisionExit(teslaRamCollisionNode, c);
				ITeslaEffectComponent effectComponent = teslaRamCollisionNode.effectComponent;
				effectComponent.triggerExit.Dispatch(ref weaponId);
			}
		}

		private void CollisionEnter(TeslaRamCollisionNode teslaRam, Collider other, Vector3 hitPoint)
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			IHardwareOwnerComponent weaponOwnerComponent = teslaRam.weaponOwnerComponent;
			MachineWeaponsBlockedNode machineWeaponsBlockedNode = default(MachineWeaponsBlockedNode);
			if (weaponOwnerComponent.ownedByMe && entityViewsDB.TryQueryEntityView<MachineWeaponsBlockedNode>(weaponOwnerComponent.machineId, ref machineWeaponsBlockedNode) && machineWeaponsBlockedNode.machineWeaponsBlockedComponent.blocked)
			{
				return;
			}
			TargetType type = LayerToTargetType.GetType(other.get_gameObject().get_layer());
			int hitMachineId;
			if (type == TargetType.Environment)
			{
				hitPoint = other.ClosestPointOnBounds(teslaRam.transformComponent.T.get_position());
				HandleHitEnvironment(teslaRam.hitSomethingComponent, teslaRam.get_ID(), hitPoint);
			}
			else if (LayerToTargetType.IsTargetDestructible(type) && IsValidTarget(other, type, out hitMachineId, teslaRam.weaponOwnerComponent.ownerId))
			{
				ITeslaTargetComponent teslaTargetComponent = teslaRam.teslaTargetComponent;
				if (!teslaTargetComponent.hasTarget || teslaTargetComponent.machineId == hitMachineId)
				{
					teslaTargetComponent.targetColliders.Add(other);
					teslaTargetComponent.hasTarget = true;
					teslaTargetComponent.playerId = playerMachinesContainer.GetPlayerFromMachineId(type, hitMachineId);
					teslaTargetComponent.machineId = hitMachineId;
					teslaTargetComponent.targetType = type;
					teslaTargetComponent.hitObjectTransform = other.get_transform();
					teslaTargetComponent.hitPoint = hitPoint;
				}
				if (teslaTargetComponent.hasTarget && teslaTargetComponent.machineId != hitMachineId)
				{
					teslaTargetComponent.targetColliders.Add(other);
					teslaTargetComponent.playerId = playerMachinesContainer.GetPlayerFromMachineId(type, hitMachineId);
					teslaTargetComponent.machineId = hitMachineId;
					teslaTargetComponent.targetType = type;
					teslaTargetComponent.hitObjectTransform = other.get_transform();
					teslaTargetComponent.hitPoint = hitPoint;
				}
				if (teslaTargetComponent.hasTarget && teslaTargetComponent.machineId == hitMachineId)
				{
					teslaTargetComponent.hitObjectTransform = other.get_transform();
					teslaTargetComponent.hitPoint = hitPoint;
				}
			}
		}

		private void CollisionExit(TeslaRamCollisionNode teslaRam, Collider other)
		{
			TargetType type = LayerToTargetType.GetType(other.get_gameObject().get_layer());
			if (type == TargetType.Environment || !LayerToTargetType.IsTargetDestructible(type))
			{
				return;
			}
			ITeslaTargetComponent teslaTargetComponent = teslaRam.teslaTargetComponent;
			int hitMachineId;
			if (teslaTargetComponent.hasTarget && IsValidTarget(other, type, out hitMachineId, teslaRam.weaponOwnerComponent.ownerId) && teslaTargetComponent.machineId == hitMachineId)
			{
				teslaTargetComponent.targetColliders.UnorderedRemove(other);
				if (teslaTargetComponent.targetColliders.get_Count() <= 0)
				{
					teslaTargetComponent.hasTarget = false;
					teslaTargetComponent.targetColliders.FastClear();
				}
			}
		}

		private void HandleHitEnvironment(IHitSomethingComponent teslaHitComponent, int nodeId, Vector3 hitPoint)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			HitInfo value = new HitInfo(nodeId, hitPoint, Quaternion.get_identity(), Vector3.get_up());
			teslaHitComponent.hitEnvironment.Dispatch(ref value);
		}

		private bool IsValidTarget(Collider other, TargetType type, out int hitMachineId, int playerId)
		{
			GameObject gameObject = other.get_transform().get_root().get_gameObject();
			hitMachineId = machineRootContainer.GetMachineIdFromRoot(type, gameObject);
			int playerFromMachineId = playerMachinesContainer.GetPlayerFromMachineId(type, hitMachineId);
			if (type == TargetType.TeamBase)
			{
				int team = playerFromMachineId;
				return !playerTeamsContainer.IsMyTeam(team);
			}
			bool flag = playerTeamsContainer.GetPlayerTeam(type, playerId) == playerTeamsContainer.GetPlayerTeam(TargetType.Player, playerFromMachineId);
			return !flag;
		}
	}
}
