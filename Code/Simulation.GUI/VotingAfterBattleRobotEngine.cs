using Simulation.Hardware.Movement.Wheeled;
using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.GUI
{
	internal class VotingAfterBattleRobotEngine : SingleEntityViewEngine<VotingAfterBattleRobotWidgetNode>, IQueryingEntityViewEngine, IEngine
	{
		private const float ROBOT_SPAWN_OFFSET = 1000f;

		private const float IDLE_ROTATION_SPEED = 3f;

		private const float HOVER_ROTATION_SPEED = 1f;

		private int _robotCounter;

		private int _robotCameraCullingMask = 1 << GameLayers.MCUBES;

		private CameraPreviewUtility.CameraConfiguration _cameraConfig;

		private Dictionary<string, CameraPreviewUtility> _cameraPreviewUtilities = new Dictionary<string, CameraPreviewUtility>();

		private Dictionary<string, Camera> _robotCameras = new Dictionary<string, Camera>();

		private Dictionary<string, Vector2> _rotationSpeed = new Dictionary<string, Vector2>();

		[Inject]
		public MachineClusterContainer machineClusterContainer
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
		internal HealingManager healingManager
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public VotingAfterBattleRobotEngine(GameEndedObserver gameEndedObserver)
		{
			_cameraConfig = new CameraPreviewUtility.CameraConfiguration(0f, 20f, 5f, 80f, 20f, 10f, 60f, 80f, 0.5f, 11f);
			gameEndedObserver.OnGameEnded += HandleGameEnded;
		}

		void IQueryingEntityViewEngine.Ready()
		{
		}

		protected override void Add(VotingAfterBattleRobotWidgetNode node)
		{
			node.votingAfterBattleRobotWidgetComponent.PlayerName.NotifyOnValueSet((Action<int, string>)HandlePlayerNameSet);
			node.votingAfterBattleRobotWidgetComponent.IsHover.NotifyOnValueSet((Action<int, bool>)HandleHoverState);
		}

		protected override void Remove(VotingAfterBattleRobotWidgetNode node)
		{
			node.votingAfterBattleRobotWidgetComponent.PlayerName.StopNotify((Action<int, string>)HandlePlayerNameSet);
			node.votingAfterBattleRobotWidgetComponent.IsHover.StopNotify((Action<int, bool>)HandleHoverState);
		}

		private void HandlePlayerNameSet(int nodeID, string playerName)
		{
			_robotCounter++;
			TaskRunner.get_Instance().Run(SetupNewRobot(nodeID, playerName));
		}

		private IEnumerator SetupNewRobot(int nodeID, string playerName)
		{
			PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(playerName);
			TaskRunner.get_Instance().Run(HealRobotCubes(preloadedMachine));
			Vector3 newPosition = Vector3.get_one() * 1000f;
			newPosition.x *= (float)_robotCounter;
			SetRobotPosition(preloadedMachine, newPosition);
			SetRobotLayers(preloadedMachine);
			CreateRobotCamera(preloadedMachine, playerName);
			VotingAfterBattleRobotWidgetNode widget = entityViewsDB.QueryEntityView<VotingAfterBattleRobotWidgetNode>(nodeID);
			widget.votingAfterBattleRobotWidgetComponent.RobotTexture.set_value(_robotCameras[playerName].get_targetTexture());
			Vector2 rotationSpeed = Vector2.get_zero();
			rotationSpeed.x = 3f;
			_rotationSpeed[playerName] = rotationSpeed;
			_cameraPreviewUtilities[playerName] = new CameraPreviewUtility();
			while (true)
			{
				Vector3 position = preloadedMachine.machineInfo.centerTransform.get_position();
				if (!(position.y < 1000f))
				{
					break;
				}
				yield return null;
			}
			Vector3 centre = preloadedMachine.machineInfo.centerTransform.get_position();
			centre.y = 0f;
			_cameraPreviewUtilities[playerName].SetCentre(centre);
			CalculateCameraDistance(preloadedMachine, playerName);
			TaskRunner.get_Instance().Run(MoveView(playerName));
		}

		private void HandleGameEnded(bool won)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<WeaponAimNode> val = entityViewsDB.QueryEntityViews<WeaponAimNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				val.get_Item(i).weaponActiveComponent.active = false;
				if (val.get_Item(i).weaponRotationTransformsComponent.horizontalTransform != null)
				{
					val.get_Item(i).weaponRotationTransformsComponent.horizontalTransform.set_localRotation(val.get_Item(i).weaponAimingComponent.initialHorizRot);
				}
				if (val.get_Item(i).weaponRotationTransformsComponent.verticalTransform != null)
				{
					val.get_Item(i).weaponRotationTransformsComponent.verticalTransform.set_localRotation(val.get_Item(i).weaponAimingComponent.initialVertRot);
				}
				if (val.get_Item(i).weaponRotationTransformsComponent.secondVerticalTransform != null)
				{
					val.get_Item(i).weaponRotationTransformsComponent.secondVerticalTransform.set_localRotation(val.get_Item(i).weaponAimingComponent.initialVertRot);
				}
			}
			List<PreloadedMachine> allPreloadedMachines = machinePreloader.GetAllPreloadedMachines();
			for (int j = 0; j < allPreloadedMachines.Count; j++)
			{
				Rigidbody rbData = allPreloadedMachines[j].rbData;
				InactiveTerrainDetector component = rbData.GetComponent<InactiveTerrainDetector>();
				if (component != null)
				{
					component.set_enabled(false);
				}
				MachineInputWrapper component2 = rbData.GetComponent<MachineInputWrapper>();
				if (component2 != null)
				{
					component2.set_enabled(false);
				}
			}
			FasterReadOnlyList<WheelNode> val2 = entityViewsDB.QueryEntityViews<WheelNode>();
			for (int k = 0; k < val2.get_Count(); k++)
			{
				val2.get_Item(k).steeringComponent.steerable = false;
				val2.get_Item(k).steeringComponent.currentSteeringAngle = 0f;
			}
		}

		private void HandleHoverState(int nodeID, bool isHover)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			VotingAfterBattleRobotWidgetNode votingAfterBattleRobotWidgetNode = entityViewsDB.QueryEntityView<VotingAfterBattleRobotWidgetNode>(nodeID);
			string value = votingAfterBattleRobotWidgetNode.votingAfterBattleRobotWidgetComponent.PlayerName.get_value();
			Vector2 value2 = _rotationSpeed[value];
			value2.x = ((!isHover) ? 3f : 1f);
			_rotationSpeed[value] = value2;
		}

		private IEnumerator HealRobotCubes(PreloadedMachine preloadedMachine)
		{
			healingManager.FullyHealMachine(TargetType.Player, preloadedMachine.machineId);
			yield return null;
		}

		private void SetRobotPosition(PreloadedMachine preloadedMachine, Vector3 newPosition)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rbData = preloadedMachine.rbData;
			RemoteMachineTicker component = rbData.GetComponent<RemoteMachineTicker>();
			if (component != null)
			{
				component.set_enabled(false);
			}
			rbData.set_isKinematic(true);
			rbData.set_useGravity(false);
			preloadedMachine.machineBoard.SetActive(true);
			rbData.set_position(newPosition);
			rbData.set_rotation(Quaternion.get_identity());
		}

		private void SetRobotLayers(PreloadedMachine preloadedMachine)
		{
			FasterList<Renderer> allRenderers = preloadedMachine.allRenderers;
			for (int i = 0; i < allRenderers.get_Count(); i++)
			{
				allRenderers.get_Item(i).get_gameObject().set_layer(GameLayers.MCUBES);
			}
		}

		private void CalculateCameraDistance(PreloadedMachine preloadedMachine, string playerName)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			Bounds val = default(Bounds);
			bool flag = false;
			MachineCluster machineCluster = machineClusterContainer.GetMachineCluster(TargetType.Player, preloadedMachine.machineId);
			for (int i = 0; i < preloadedMachine.allCubeTransforms.get_Count(); i++)
			{
				FasterList<NodeBoxCollider> colliders = machineCluster.GetColliders();
				FasterListEnumerator<NodeBoxCollider> enumerator = colliders.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						NodeBoxCollider current = enumerator.get_Current();
						if (flag)
						{
							val.Encapsulate(current.collider.get_bounds());
						}
						else
						{
							Bounds bounds = current.collider.get_bounds();
							val.set_center(bounds.get_center());
							Bounds bounds2 = current.collider.get_bounds();
							val.set_extents(bounds2.get_extents());
							Bounds bounds3 = current.collider.get_bounds();
							val.set_max(bounds3.get_max());
							Bounds bounds4 = current.collider.get_bounds();
							val.set_min(bounds4.get_min());
							Bounds bounds5 = current.collider.get_bounds();
							val.set_size(bounds5.get_size());
							flag = true;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
			}
			_cameraPreviewUtilities[playerName].SetMachineSize(val.get_min(), val.get_max());
			float num = Vector3.Distance(val.get_min(), val.get_max());
			if (num > _cameraConfig.distanceFromTargetMinimum)
			{
				_cameraConfig = new CameraPreviewUtility.CameraConfiguration(num, _cameraConfig.distanceFromTargetMaximum, _cameraConfig.pitchMin, _cameraConfig.pitchMax, _cameraConfig.zoomSpeed, _cameraConfig.draggingRotationSpeed, _cameraConfig.FOVAtMaxZoomOut, _cameraConfig.FOVNormal, _cameraConfig.distanceToNearestWall, _cameraConfig.zoomLevelToBeginHuggingWall);
				foreach (KeyValuePair<string, CameraPreviewUtility> cameraPreviewUtility in _cameraPreviewUtilities)
				{
					cameraPreviewUtility.Value.SetConfig(_cameraConfig);
				}
			}
			else
			{
				_cameraPreviewUtilities[playerName].SetConfig(_cameraConfig);
			}
		}

		private void CreateRobotCamera(PreloadedMachine preloadedMachine, string playerName)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Expected O, but got Unknown
			_robotCameras[playerName] = new GameObject().AddComponent<Camera>();
			_robotCameras[playerName].set_clearFlags(2);
			_robotCameras[playerName].set_backgroundColor(Color.get_clear());
			_robotCameras[playerName].set_depth(-10f);
			_robotCameras[playerName].set_targetTexture(new RenderTexture(512, 512, 16, 0));
			_robotCameras[playerName].get_targetTexture().Create();
			_robotCameras[playerName].set_cullingMask(_robotCameraCullingMask);
		}

		private IEnumerator MoveView(string playerName)
		{
			while (true)
			{
				float currentPitch2 = _cameraPreviewUtilities[playerName].GetCurrentPitch();
				Vector2 val = _rotationSpeed[playerName];
				float num;
				if (val.y < 0f)
				{
					CameraPreviewUtility.CameraConfiguration value = _cameraPreviewUtilities[playerName].GetConfig().Value;
					num = value.pitchMax;
				}
				else
				{
					CameraPreviewUtility.CameraConfiguration value2 = _cameraPreviewUtilities[playerName].GetConfig().Value;
					num = value2.pitchMin;
				}
				Vector2 val2 = _rotationSpeed[playerName];
				float num2 = Mathf.Abs(val2.y) * Time.get_deltaTime();
				CameraPreviewUtility.CameraConfiguration value3 = _cameraPreviewUtilities[playerName].GetConfig().Value;
				float currentPitch = Mathf.MoveTowards(currentPitch2, num, num2 * value3.draggingRotationSpeed);
				_cameraPreviewUtilities[playerName].SetCurrentPitch(currentPitch);
				float currentYaw2 = _cameraPreviewUtilities[playerName].GetCurrentYaw();
				float num3 = currentYaw2;
				Vector2 val3 = _rotationSpeed[playerName];
				float num4 = val3.x * Time.get_deltaTime();
				CameraPreviewUtility.CameraConfiguration value4 = _cameraPreviewUtilities[playerName].GetConfig().Value;
				currentYaw2 = num3 + num4 * value4.draggingRotationSpeed;
				while (360f < currentYaw2)
				{
					currentYaw2 -= 360f;
				}
				for (; currentYaw2 < 0f; currentYaw2 += 360f)
				{
				}
				_cameraPreviewUtilities[playerName].SetCurrentYaw(currentYaw2);
				HandleCameraDataChanged(playerName);
				yield return null;
			}
		}

		private void HandleCameraDataChanged(string playerName)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			Vector3 calculatedPosition;
			float num = _cameraPreviewUtilities[playerName].RecalculateCameraPosition(_robotCameras[playerName], out calculatedPosition);
			_robotCameras[playerName].get_transform().set_position(calculatedPosition);
			_robotCameras[playerName].get_transform().LookAt(_cameraPreviewUtilities[playerName].CalculateCameraTarget());
			_cameraPreviewUtilities[playerName].UpdateCameraFOV(_robotCameras[playerName], revertToNormal: false);
		}
	}
}
