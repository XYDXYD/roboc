using RCNetwork.Events;
using Simulation.Hardware.Modules;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class TeleportModuleTeleporterEngine : MultiEntityViewsEngine<TeleportModuleTeleporterNode, CameraControlNode>, IQueryingEntityViewEngine, ITickable, IInitialize, IWaitForFrameworkDestruction, IEngine, ITickableBase
	{
		private TeleportActivateEffectDependency _teleportActivateEffectDependency = new TeleportActivateEffectDependency();

		private Vector3 _inertiaTensor;

		private ICameraControlComponent _cameraControlComponent;

		private Dictionary<int, Vector3> originalVelocityPerPlayer = new Dictionary<int, Vector3>();

		private Dictionary<int, Vector3> originalAngularVelocityPerPlayer = new Dictionary<int, Vector3>();

		private Dictionary<int, Vector3> inertiaTensorPerPlayer = new Dictionary<int, Vector3>();

		[Inject]
		internal INetworkEventManagerClient networkManager
		{
			get;
			set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(TeleportModuleTeleporterNode node)
		{
			node.teleporterComponent.teleportStarted.subscribers += HandleStartTeleport;
		}

		protected override void Remove(TeleportModuleTeleporterNode node)
		{
			node.teleporterComponent.teleportStarted.subscribers -= HandleStartTeleport;
		}

		protected override void Add(CameraControlNode node)
		{
			_cameraControlComponent = node.cameraControlComponent;
		}

		protected override void Remove(CameraControlNode node)
		{
		}

		public void OnDependenciesInjected()
		{
			destructionReporter.OnMachineDestroyed += HandleOnMachineDestroyed;
		}

		public void OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineDestroyed -= HandleOnMachineDestroyed;
		}

		private void HandleOnMachineDestroyed(int ownerId, int machineId, bool isMe)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			if (!isMe)
			{
				return;
			}
			int num = default(int);
			TeleportModuleTeleporterNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<TeleportModuleTeleporterNode>(machineId, ref num);
			int num2 = 0;
			TeleportModuleTeleporterNode teleportModuleTeleporterNode;
			while (true)
			{
				if (num2 < num)
				{
					teleportModuleTeleporterNode = array[num2];
					if (teleportModuleTeleporterNode.teleporterComponent.teleportActivated)
					{
						break;
					}
					num2++;
					continue;
				}
				return;
			}
			Vector3 destination = teleportModuleTeleporterNode.teleporterComponent.destination;
			Rigidbody rb = teleportModuleTeleporterNode.rigidbodyComponent.rb;
			Vector3 val = destination - rb.get_position();
			Vector3 normalized = val.get_normalized();
			Vector3 lastCameraPosition = _cameraControlComponent.lastCameraPosition;
			float teleportTimer = teleportModuleTeleporterNode.teleporterComponent.teleportTimer;
			float distance = teleportModuleTeleporterNode.teleporterComponent.distance;
			ITeleportModuleSettingsComponent settingsComponent = teleportModuleTeleporterNode.settingsComponent;
			TeleportFinish(teleportModuleTeleporterNode, machineId, rb, normalized, lastCameraPosition, teleportTimer, distance, settingsComponent);
		}

		void ITickable.Tick(float deltaSec)
		{
			UpdateTeleportingPlayers();
		}

		private void UpdateTeleportingPlayers()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<TeleportModuleTeleporterNode> val = entityViewsDB.QueryEntityViews<TeleportModuleTeleporterNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				TeleportModuleTeleporterNode teleportModuleTeleporterNode = val.get_Item(i);
				if (teleportModuleTeleporterNode.ownerComponent.ownedByMe && teleportModuleTeleporterNode.teleporterComponent.teleportActivated)
				{
					int machineId = teleportModuleTeleporterNode.ownerComponent.machineId;
					Vector3 destination = teleportModuleTeleporterNode.teleporterComponent.destination;
					Rigidbody rb = teleportModuleTeleporterNode.rigidbodyComponent.rb;
					Vector3 val2 = destination - rb.get_position();
					Vector3 normalized = val2.get_normalized();
					Vector3 lastCameraPosition = _cameraControlComponent.lastCameraPosition;
					float teleportTimer = teleportModuleTeleporterNode.teleporterComponent.teleportTimer;
					ITeleportModuleSettingsComponent settingsComponent = teleportModuleTeleporterNode.settingsComponent;
					float teleportTime = settingsComponent.teleportTime;
					float distance = teleportModuleTeleporterNode.teleporterComponent.distance;
					if (Vector3.SqrMagnitude(destination - rb.get_position()) > 0.0100000007f && teleportTimer < teleportTime)
					{
						TeleportPlayer(teleportModuleTeleporterNode, destination, rb, normalized, lastCameraPosition, settingsComponent, teleportTime, distance);
					}
					else
					{
						TeleportFinish(teleportModuleTeleporterNode, machineId, rb, normalized, lastCameraPosition, teleportTimer, distance, settingsComponent);
					}
				}
			}
		}

		private void TeleportPlayer(TeleportModuleTeleporterNode teleportNode, Vector3 destination, Rigidbody rb, Vector3 direction, Vector3 lastCameraPosition, ITeleportModuleSettingsComponent teleportSettingsData, float teleportTime, float distance)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			rb.set_position(Vector3.MoveTowards(rb.get_position(), destination, distance / teleportTime * Time.get_deltaTime()));
			teleportNode.teleporterComponent.teleportTimer += Time.get_deltaTime();
			float teleportTimer = teleportNode.teleporterComponent.teleportTimer;
			if (teleportTimer >= teleportSettingsData.cameraDelay && !_cameraControlComponent.activateProgressiveFollow)
			{
				ActivateCameraProgressiveFollow(direction, lastCameraPosition, teleportSettingsData, distance);
			}
		}

		private void TeleportFinish(TeleportModuleTeleporterNode teleportNode, int machineId, Rigidbody rb, Vector3 direction, Vector3 lastCameraPosition, float timer, float distance, ITeleportModuleSettingsComponent teleportSettingsData)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			TeleportEnd(rb, direction, originalVelocityPerPlayer[machineId], originalAngularVelocityPerPlayer[machineId], inertiaTensorPerPlayer[machineId], lastCameraPosition, timer, teleportSettingsData, distance);
			teleportNode.teleporterComponent.teleportTimer = 0f;
			teleportNode.teleporterComponent.teleportActivated = false;
			int value = teleportNode.get_ID();
			teleportNode.teleporterComponent.teleportEnded.Dispatch(ref value);
			_teleportActivateEffectDependency.Inject(activate_: false, teleportNode.ownerComponent.ownerId, teleportNode.get_ID());
			networkManager.SendEventToServer(NetworkEvent.BroadcastActivateTeleportEffect, _teleportActivateEffectDependency);
		}

		private void TeleportEnd(Rigidbody rb, Vector3 direction, Vector3 originalVelocity, Vector3 originalAngularVelocity, Vector3 inertiaTensor, Vector3 lastCameraPosition, float timer, ITeleportModuleSettingsComponent teleportSettingsData, float distance)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (timer < teleportSettingsData.cameraDelay)
			{
				ActivateCameraProgressiveFollow(direction, lastCameraPosition, teleportSettingsData, distance);
			}
			if (!Physics.Raycast(rb.get_worldCenterOfMass(), originalVelocity.get_normalized(), 8f, GameLayers.ENVIRONMENT_LAYER_MASK))
			{
				rb.set_velocity(originalVelocity);
				rb.set_angularVelocity(originalAngularVelocity);
			}
			rb.set_detectCollisions(true);
			rb.set_constraints(0);
			rb.set_inertiaTensor(inertiaTensor);
		}

		private void ActivateCameraProgressiveFollow(Vector3 direction, Vector3 lastCameraPosition, ITeleportModuleSettingsComponent teleportSettingsData, float distance)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			_cameraControlComponent.instantFollowEnabled = false;
			_cameraControlComponent.activateProgressiveFollow = true;
			_cameraControlComponent.cameraTime = teleportSettingsData.cameraTime;
			Vector3 finalExpectedCameraPosition = lastCameraPosition + direction * distance;
			_cameraControlComponent.finalExpectedCameraPosition = finalExpectedCameraPosition;
		}

		private void HandleStartTeleport(ITeleporterComponent sender, int moduleId)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			TeleportModuleTeleporterNode teleportModuleTeleporterNode = default(TeleportModuleTeleporterNode);
			if (entityViewsDB.TryQueryEntityView<TeleportModuleTeleporterNode>(moduleId, ref teleportModuleTeleporterNode))
			{
				int machineId = teleportModuleTeleporterNode.ownerComponent.machineId;
				Rigidbody rb = teleportModuleTeleporterNode.rigidbodyComponent.rb;
				originalVelocityPerPlayer[machineId] = rb.get_velocity();
				originalAngularVelocityPerPlayer[machineId] = rb.get_angularVelocity();
				rb.set_detectCollisions(false);
				inertiaTensorPerPlayer[machineId] = rb.get_inertiaTensor();
				rb.set_constraints(112);
				if (teleportModuleTeleporterNode.ownerComponent.ownedByMe)
				{
					_cameraControlComponent.instantFollowEnabled = false;
				}
			}
		}
	}
}
