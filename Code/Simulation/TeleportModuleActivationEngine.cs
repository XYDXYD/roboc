using RCNetwork.Events;
using Simulation.Hardware.Modules;
using Simulation.Hardware.Movement.Rotors;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class TeleportModuleActivationEngine : SingleEntityViewEngine<TeleportModuleActivationNode>, IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IEngine
	{
		private const float GROUND_CLEARANCE = 0.2f;

		private const float RAYCAST_DIST = 16f;

		private const float RADIUS_FACTOR = 2.5f;

		private const float DISTANCE_THRESHOLD = 6f;

		private Camera _mainCamera;

		private Vector3 _middleScreenPoint;

		private bool _raycastTerrain;

		private bool _setJumpState;

		private readonly Dictionary<int, float> _overlapSpereRadiusPerMachine = new Dictionary<int, float>();

		private readonly TeleportActivateEffectDependency _teleportActivateEffectDependency = new TeleportActivateEffectDependency();

		[Inject]
		internal INetworkEventManagerClient networkManager
		{
			get;
			set;
		}

		[Inject]
		internal LegController legController
		{
			private get;
			set;
		}

		[Inject]
		internal MechLegController mechLegController
		{
			private get;
			set;
		}

		[Inject]
		internal CeilingHeightManager ceilingHeightManager
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		protected override void Add(TeleportModuleActivationNode node)
		{
			node.activationComponent.activate.subscribers += HandleActivateTeleport;
		}

		protected override void Remove(TeleportModuleActivationNode node)
		{
			node.activationComponent.activate.subscribers -= HandleActivateTeleport;
		}

		public void Ready()
		{
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			_middleScreenPoint = new Vector3((float)(Screen.get_width() / 2), (float)(Screen.get_height() / 2), 1f);
			_mainCamera = Camera.get_main();
		}

		private void HandleActivateTeleport(IModuleActivationComponent sender, int moduleId)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
			TeleportModuleActivationNode teleportModuleActivationNode = default(TeleportModuleActivationNode);
			if (!entityViewsDB.TryQueryEntityView<TeleportModuleActivationNode>(moduleId, ref teleportModuleActivationNode))
			{
				return;
			}
			Ray ray = _mainCamera.ScreenPointToRay(_middleScreenPoint);
			Vector3 machineSize = teleportModuleActivationNode.machineSizeComponent.machineSize;
			int machineId = teleportModuleActivationNode.ownerComponent.machineId;
			if (!_overlapSpereRadiusPerMachine.TryGetValue(machineId, out float value))
			{
				value = machineSize.x;
				if (machineSize.z > value)
				{
					value = machineSize.z;
				}
				if (machineSize.y > value)
				{
					value = machineSize.y;
				}
				value /= 2f;
				_overlapSpereRadiusPerMachine.Add(machineId, value);
			}
			Rigidbody rb = teleportModuleActivationNode.rigidbodyComponent.rb;
			Vector3 destinationNormal = Vector3.get_up();
			Quaternion playerRotation = rb.get_rotation();
			RaycastToFindDestination(teleportModuleActivationNode, ray, machineSize, value, rb, out Vector3 destination, out Vector3 overlapSpherePoint, ref destinationNormal, ref playerRotation, out RaycastHit hitInfo, out RaycastHit hit, out bool rayCastHit, out bool hitGround);
			float maxCeilingHeight = ceilingHeightManager.GetMaxCeilingHeight();
			float distance = Vector3.Distance(rb.get_position(), destination);
			LocalMachineRotorNode localMachineRotorNode = default(LocalMachineRotorNode);
			float num = entityViewsDB.TryQueryEntityView<LocalMachineRotorNode>(machineId, ref localMachineRotorNode) ? (destination.y * localMachineRotorNode.averageMovementValuesComponent.avgCeilingHeightModifier) : destination.y;
			if (num > maxCeilingHeight)
			{
				float num2 = num - maxCeilingHeight;
				destination.y -= num2;
				distance = Vector3.Distance(rb.get_position(), destination);
			}
			Vector3 val = destination - rb.get_position();
			Vector3 direction = val.get_normalized();
			CheckIfDestinationIsFree(value, rb, ref destination, ref overlapSpherePoint, hitInfo, hit, rayCastHit, hitGround, ref direction, ref distance);
			if (distance > 6f)
			{
				rb.set_rotation(playerRotation);
				if (_setJumpState)
				{
					legController.SetJumpStateForLocalLegs();
					mechLegController.SetJumpStateForLocalMechLegs();
					_setJumpState = false;
				}
				teleportModuleActivationNode.startTeleportComponent.destination = destination;
				teleportModuleActivationNode.startTeleportComponent.distance = distance;
				teleportModuleActivationNode.startTeleportComponent.teleportActivated = true;
				teleportModuleActivationNode.startTeleportComponent.teleportStarted.Dispatch(ref moduleId);
				_teleportActivateEffectDependency.Inject(activate_: true, teleportModuleActivationNode.ownerComponent.ownerId, moduleId);
				networkManager.SendEventToServer(NetworkEvent.BroadcastActivateTeleportEffect, _teleportActivateEffectDependency);
				teleportModuleActivationNode.confirmActivationComponent.activationConfirmed.Dispatch(ref moduleId);
			}
		}

		private void CheckIfDestinationIsFree(float overlapSphereRadius, Rigidbody rb, ref Vector3 destination, ref Vector3 overlapSpherePoint, RaycastHit hitInfo, RaycastHit hit, bool rayCastHit, bool hitGround, ref Vector3 direction, ref float distance)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			Collider[] hitColliders;
			int num = RaycastUtility.OverlapSphere(ref overlapSpherePoint, overlapSphereRadius, GameLayers.SHOOTING_OTHERS_LAYER_MASK, out hitColliders);
			if (num <= 0)
			{
				return;
			}
			if (rayCastHit)
			{
				if (hitGround)
				{
					Vector3 val = destination - (rb.get_position() + hit.get_normal() * overlapSphereRadius * 2.5f);
					direction = val.get_normalized();
				}
				else
				{
					Vector3 val2 = destination - (rb.get_position() + hitInfo.get_normal() * overlapSphereRadius * 2.5f);
					direction = val2.get_normalized();
				}
			}
			else if (_raycastTerrain)
			{
				Vector3 val3 = destination - (rb.get_position() + Vector3.get_up() * overlapSphereRadius * 2.5f);
				direction = val3.get_normalized();
			}
			while (num > 0 && distance > 0f)
			{
				destination -= direction * overlapSphereRadius * 2.5f;
				distance -= overlapSphereRadius * 2.5f;
				overlapSpherePoint = destination;
				num = RaycastUtility.OverlapSphere(ref overlapSpherePoint, overlapSphereRadius, GameLayers.SHOOTING_OTHERS_LAYER_MASK, out hitColliders);
			}
		}

		private void RaycastToFindDestination(TeleportModuleActivationNode teleportActivationNode, Ray ray, Vector3 machineSize, float overlapSphereRadius, Rigidbody rb, out Vector3 destination, out Vector3 overlapSpherePoint, ref Vector3 destinationNormal, ref Quaternion playerRotation, out RaycastHit hitInfo, out RaycastHit hit, out bool rayCastHit, out bool hitGround)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			hit = default(RaycastHit);
			float moduleRange = teleportActivationNode.rangeComponent.moduleRange;
			hitGround = false;
			RecursiveDestinationLookup(ray, out hitInfo, moduleRange);
			if (hitInfo.get_collider() != null)
			{
				rayCastHit = true;
				OnHitSomething(teleportActivationNode, machineSize, overlapSphereRadius, rb, out destination, out overlapSpherePoint, out destinationNormal, ref playerRotation, ref hitInfo, out hit, out hitGround);
			}
			else
			{
				rayCastHit = false;
				OnHitNothing(ray, overlapSphereRadius, out destination, out overlapSpherePoint, moduleRange);
			}
		}

		private void RecursiveDestinationLookup(Ray ray, out RaycastHit hitInfo, float range)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			if (Physics.Raycast(ray, ref hitInfo, range, GameLayers.TELEPORT_LAYER_MASK, 2) && (hitInfo.get_collider().get_tag() == WeaponRaycastUtility.ALLY_FUSION_SHIELD_TAG || hitInfo.get_collider().get_tag() == WeaponRaycastUtility.ENEMY_FUSION_SHIELD_TAG))
			{
				GameObject gameObject = hitInfo.get_collider().get_gameObject();
				int layer = hitInfo.get_collider().get_gameObject().get_layer();
				hitInfo.get_collider().get_gameObject().set_layer(GameLayers.IGNORE_RAYCAST);
				RecursiveDestinationLookup(ray, out hitInfo, range);
				gameObject.set_layer(layer);
			}
		}

		private void OnHitSomething(TeleportModuleActivationNode teleportActivationNode, Vector3 machineSize, float overlapSphereRadius, Rigidbody rb, out Vector3 destination, out Vector3 overlapSpherePoint, out Vector3 destinationNormal, ref Quaternion playerRotation, ref RaycastHit hitInfo, out RaycastHit hit, out bool hitGround)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_019a: Unknown result type (might be due to invalid IL or missing references)
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0205: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0215: Unknown result type (might be due to invalid IL or missing references)
			//IL_021a: Unknown result type (might be due to invalid IL or missing references)
			//IL_021f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			//IL_022d: Unknown result type (might be due to invalid IL or missing references)
			//IL_023b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0242: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			//IL_0252: Unknown result type (might be due to invalid IL or missing references)
			//IL_0257: Unknown result type (might be due to invalid IL or missing references)
			//IL_025c: Unknown result type (might be due to invalid IL or missing references)
			destination = hitInfo.get_point();
			destinationNormal = hitInfo.get_normal();
			Vector3 val = destination + Vector3.get_up() * machineSize.y;
			Vector3 centerOfMass = rb.get_centerOfMass();
			Vector3 machineCenter = teleportActivationNode.machineSizeComponent.machineCenter;
			float num = machineCenter.y - machineSize.y * 0.5f;
			float num2 = centerOfMass.y - num;
			centerOfMass.y -= 0.2f + num2;
			hitGround = Physics.Raycast(val, Vector3.get_down(), ref hit, 16f, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
			if (Vector3.Angle(Vector3.get_up(), hitInfo.get_normal()) > 90f)
			{
				hitGround = false;
			}
			destination -= playerRotation * centerOfMass;
			if (hitGround)
			{
				Quaternion val2 = Quaternion.FromToRotation(Vector3.get_up(), hit.get_normal());
				playerRotation = val2 * rb.get_rotation();
				destination = hit.get_point();
				destinationNormal = hit.get_normal();
				destination -= playerRotation * centerOfMass;
				if (Vector3.Angle(rb.get_transform().get_up(), hit.get_normal()) > 30f)
				{
					destination += hit.get_normal() * overlapSphereRadius * 2.5f;
					overlapSpherePoint = hit.get_point();
				}
				else
				{
					overlapSpherePoint = hit.get_point() + hit.get_normal() * overlapSphereRadius * 1.1f;
				}
			}
			else if (Vector3.Angle(rb.get_transform().get_up(), hitInfo.get_normal()) > 30f)
			{
				destination += hitInfo.get_normal() * overlapSphereRadius * 2.5f;
				overlapSpherePoint = hitInfo.get_point();
			}
			else
			{
				overlapSpherePoint = hitInfo.get_point() + hitInfo.get_normal() * overlapSphereRadius * 1.1f;
			}
		}

		private void OnHitNothing(Ray ray, float overlapSphereRadius, out Vector3 destination, out Vector3 overlapSpherePoint, float range)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			destination = ray.get_origin() + ray.get_direction() * range;
			if (Physics.Raycast(destination, Vector3.get_down(), overlapSphereRadius, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK))
			{
				overlapSpherePoint = destination + Vector3.get_up() * overlapSphereRadius * 2.5f;
				_raycastTerrain = true;
			}
			else
			{
				overlapSpherePoint = destination;
				_raycastTerrain = false;
				_setJumpState = true;
			}
		}
	}
}
