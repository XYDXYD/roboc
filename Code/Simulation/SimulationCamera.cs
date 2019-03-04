using Simulation.Hardware.Weapons;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal class SimulationCamera : MonoBehaviour, IInitialize, IHandleCharacterInput, IInputComponent, IComponent
	{
		private MachineInfo _machineInfo;

		private List<MachineInfo> _playerMachines = new List<MachineInfo>();

		public float cameraCollisionRadius = 0.7f;

		public float minMouseSpeedY = 0.2f;

		public float maxMouseSpeedY = 2f;

		public float minMouseSpeedX = 0.6f;

		public float maxMouseSpeedX = 6f;

		public float maxYAngle = 80f;

		public float minYAngle = 20f;

		public float mouse_x_scale = 3f;

		public float mouse_y_scale = 1f;

		public float mouseSensitivityZoomedScale = 0.75f;

		public int initialCameraLevel;

		public int numberCameraLevels = 3;

		public float minCameraDistance = 0.2f;

		public float maxCameraDistance = 3f;

		public float cinematicDistanceScale = 1.5f;

		public float minHeightOffset = 0.4f;

		public float zoomedOutHeightOffset;

		public float cameraTransitionTime = 0.4f;

		public float acceleratingInterpolationTime = 2f;

		public float velocityDamper = 0.1f;

		public float velocityTolerance = 8f;

		public float deceleratingInterpolationTime = 1f;

		public float velClamp = 20f;

		public Transform cameraParent;

		internal Transform T;

		protected Transform _TrackingCube;

		private bool _instantFollow = true;

		private bool _progressiveFollow;

		private float _cameraTime;

		private float _timer;

		private Vector3 _lastCameraPositionToBlinkFrom;

		private Vector3 _expectedCameraPosition;

		private float _collisionRadiusFactor = 1f;

		private bool _receivedNewInputs;

		private int _usedNumberCameraLevels = 1;

		private bool _doUpdateInput = true;

		private bool _startingPosInitialized;

		private float _vTargetAngle = -20f;

		private bool _zoomed;

		private float _zoomAmount = 1f;

		private int _distanceIndex;

		private float[] _cameraDistances;

		private Vector3 CubeOffsetMax = new Vector3(0f, 0.6f, 0f);

		private Vector3 CubeOffsetMin = new Vector3(0f, 0.4f, 0f);

		private float _vAngle = -20f;

		private float _hAngle = 180f;

		private float CameraDistance = 2f;

		private float _oldDistanceScale;

		private float _newDistanceScale;

		private float _distanceTransitionTime;

		private float _distanceScale;

		private float _oldDistance;

		private float _mouseX;

		private float _mouseY;

		private Vector3 _currentSpeed;

		private Rigidbody _rigidBody;

		private float _lastModule;

		private bool _isInputAllowed = true;

		[Inject]
		internal ICubeList cubeList
		{
			private get;
			set;
		}

		[Inject]
		internal ICursorMode cursorMode
		{
			private get;
			set;
		}

		[Inject]
		internal LobbyGameStartPresenter lobbyGameStartPesenter
		{
			private get;
			set;
		}

		[Inject]
		internal ZoomEngine zoomMode
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
		internal IMinimapPresenter minimapPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal MouseSettings mouseSettings
		{
			private get;
			set;
		}

		protected Vector3 cameraPivotPoint => _TrackingCube.get_position() + (CubeOffsetMin * (1f - _distanceScale) + CubeOffsetMax * _distanceScale);

		public bool isInputAllowed
		{
			set
			{
				_isInputAllowed = value;
			}
		}

		public SimulationCamera()
			: this()
		{
		}//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)


		public virtual void OnDependenciesInjected()
		{
			LobbyGameStartPresenter lobbyGameStartPesenter = this.lobbyGameStartPesenter;
			lobbyGameStartPesenter.OnInitialLobbyGuiClose = (Action)Delegate.Combine(lobbyGameStartPesenter.OnInitialLobbyGuiClose, new Action(SetUpCursorModeInputToggle));
			zoomMode.OnZoomModeChange += ZoomChanged;
			spawnDispatcher.OnPlayerRegistered += HandleOnPlayerRegistered;
			minimapPresenter.RegisterPlayerCamera(this);
		}

		private void OnDestroy()
		{
			zoomMode.OnZoomModeChange -= ZoomChanged;
		}

		private void ZoomChanged(ZoomType zoomType, float zoomAmount)
		{
			_zoomed = (zoomType != ZoomType.NoZoom);
			_zoomAmount = zoomAmount;
		}

		private void HandleOnPlayerRegistered(SpawnInParametersPlayer data)
		{
			if (data.isMe)
			{
				if (_machineInfo == null)
				{
					_machineInfo = data.preloadedMachine.machineInfo;
					TaskRunner.get_Instance().Run((Func<IEnumerator>)DelayedStartTracking);
				}
				_playerMachines.Add(data.preloadedMachine.machineInfo);
			}
		}

		private IEnumerator DelayedStartTracking()
		{
			yield return (object)new WaitForSecondsEnumerator(1f);
		}

		private void SetUpCursorModeInputToggle()
		{
			LobbyGameStartPresenter lobbyGameStartPesenter = this.lobbyGameStartPesenter;
			lobbyGameStartPesenter.OnInitialLobbyGuiClose = (Action)Delegate.Remove(lobbyGameStartPesenter.OnInitialLobbyGuiClose, new Action(SetUpCursorModeInputToggle));
			cursorMode.OnSwitch += ToggleUpdateInput;
		}

		private void ToggleUpdateInput(Mode cursorMode)
		{
			_doUpdateInput = (cursorMode == Mode.Lock);
		}

		private void Awake()
		{
			if (numberCameraLevels == 0)
			{
				numberCameraLevels = 1;
			}
			_usedNumberCameraLevels = numberCameraLevels;
			_cameraDistances = new float[_usedNumberCameraLevels + 1];
			for (int i = 0; i < _usedNumberCameraLevels; i++)
			{
				_cameraDistances[i] = 1f;
			}
			T = this.get_transform();
		}

		private void OnEnable()
		{
			_startingPosInitialized = false;
			ResetAngles();
			if (_TrackingCube != null)
			{
				ComputeOffsets(_machineInfo);
			}
		}

		protected virtual void Start()
		{
			if (_TrackingCube == null)
			{
				ResetAngles();
			}
			InitializeCam();
			mouseSettings.OnChangeMouseSettings += HandleOnChangeMouseSettings;
		}

		private void InitializeCam()
		{
			mouse_x_scale = mouseSettings.GetFightSpeed() * (maxMouseSpeedX - minMouseSpeedX) + minMouseSpeedX;
			mouse_y_scale = mouseSettings.GetFightSpeed() * (maxMouseSpeedY - minMouseSpeedY) + minMouseSpeedY;
			if (mouseSettings.IsInvertY())
			{
				mouse_y_scale = 0f - mouse_y_scale;
			}
			GetTrackingCube();
			if (_TrackingCube != null)
			{
				ComputeOffsets(_machineInfo);
			}
			CameraDistance = _cameraDistances[_distanceIndex];
		}

		private void HandleOnChangeMouseSettings(float build, float fight, bool invert)
		{
			mouse_x_scale = fight * (maxMouseSpeedX - minMouseSpeedX) + minMouseSpeedX;
			mouse_y_scale = fight * (maxMouseSpeedY - minMouseSpeedY) + minMouseSpeedY;
			if (invert)
			{
				mouse_y_scale = 0f - mouse_y_scale;
			}
		}

		protected void ComputeOffsets(MachineInfo info)
		{
			float num = maxCameraDistance;
			float num2 = zoomedOutHeightOffset;
			float num3 = num;
			_usedNumberCameraLevels = numberCameraLevels;
			if (_usedNumberCameraLevels > 1)
			{
				num3 = (num - minCameraDistance) / (float)(_usedNumberCameraLevels - 1);
			}
			for (int i = 0; i < _usedNumberCameraLevels; i++)
			{
				_cameraDistances[i] = minCameraDistance + num3 * (float)i;
			}
			_cameraDistances[_usedNumberCameraLevels] = cinematicDistanceScale * _cameraDistances[_usedNumberCameraLevels - 1];
			_usedNumberCameraLevels++;
			CubeOffsetMin.y = info.MachineSize.y * 0.5f + minHeightOffset;
			CubeOffsetMax.y = info.MachineSize.y * 0.5f + num2 + (num - minCameraDistance) * Mathf.Tan(Camera.get_main().get_fieldOfView());
			_distanceIndex = Mathf.Clamp(initialCameraLevel, 0, _usedNumberCameraLevels - 1);
		}

		private void ResetAngles()
		{
			_vAngle = 0f;
			_hAngle = 180f;
			_distanceIndex = initialCameraLevel;
			_vTargetAngle = _vAngle;
		}

		private void GetTrackingCube()
		{
			int num = 0;
			MachineInfo machineInfo;
			while (true)
			{
				if (num < _playerMachines.Count)
				{
					machineInfo = _playerMachines[num];
					if (machineInfo != null && machineInfo.cameraPivotTransform != null && machineInfo.cameraPivotTransform.get_gameObject().get_activeInHierarchy())
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_machineInfo = machineInfo;
			_TrackingCube = _machineInfo.cameraPivotTransform;
			_rigidBody = _TrackingCube.GetComponentInParent<Rigidbody>();
		}

		private void InitializeStartingPos()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			if (_TrackingCube != null && _TrackingCube.get_gameObject().get_activeInHierarchy())
			{
				_startingPosInitialized = true;
				Quaternion rotation = _TrackingCube.get_rotation();
				Vector3 eulerAngles = rotation.get_eulerAngles();
				_hAngle = eulerAngles.y + 180f;
			}
		}

		public virtual void HandleCharacterInput(InputCharacterData data)
		{
			if (!_isInputAllowed)
			{
				return;
			}
			if (_instantFollow)
			{
				_receivedNewInputs = true;
				if (!_zoomed)
				{
					float num = data.data[8];
					if (num > 0f)
					{
						_distanceIndex--;
					}
					else if (num < 0f)
					{
						_distanceIndex++;
					}
					_distanceIndex = Mathf.Clamp(_distanceIndex, 0, _usedNumberCameraLevels - 1);
					if (_usedNumberCameraLevels > 1)
					{
						_newDistanceScale = (float)_distanceIndex / (float)(_usedNumberCameraLevels - 1);
					}
					else
					{
						_newDistanceScale = 0f;
					}
					_oldDistanceScale = _distanceScale;
					_distanceTransitionTime = cameraTransitionTime;
					_oldDistance = CameraDistance;
				}
				_mouseX = data.data[4];
				_mouseY = data.data[5];
			}
			else
			{
				_mouseX = 0f;
				_mouseY = 0f;
			}
		}

		protected virtual void Update()
		{
			if (_TrackingCube == null || !_TrackingCube.get_gameObject().get_activeInHierarchy())
			{
				InitializeCam();
			}
			if (!_startingPosInitialized)
			{
				InitializeStartingPos();
			}
			if (_doUpdateInput && _receivedNewInputs)
			{
				UpdateInputControls();
			}
			TansitionCameraDistance();
			if (_TrackingCube != null)
			{
				if (_zoomed)
				{
					PositionFirstPersonCamera();
				}
				else
				{
					PositionThirdPersonCamera();
				}
			}
			_receivedNewInputs = false;
		}

		private void TansitionCameraDistance()
		{
			if (_distanceTransitionTime > 0f)
			{
				_distanceTransitionTime = Mathf.Clamp(_distanceTransitionTime - Time.get_deltaTime(), 0f, cameraTransitionTime);
				float num = 1f;
				if (cameraTransitionTime > 0f)
				{
					num = _distanceTransitionTime / cameraTransitionTime;
				}
				float num2 = Mathf.Sqrt(1f - num * num);
				_distanceScale = _oldDistanceScale * (1f - num2) + _newDistanceScale * num2;
				if (_cameraDistances.Length > 0)
				{
					CameraDistance = _oldDistance * (1f - num2) + _cameraDistances[_distanceIndex] * num2;
				}
			}
		}

		private void UpdateInputControls()
		{
			float num = 1f;
			if (_zoomed)
			{
				num /= _zoomAmount;
				num *= mouseSensitivityZoomedScale;
			}
			_hAngle += _mouseX * mouse_x_scale * num;
			_vTargetAngle += _mouseY * mouse_y_scale * num;
			if (Mathf.Abs(_hAngle) > 360f)
			{
				_hAngle -= 360f * Mathf.Sign(_hAngle);
			}
			_vTargetAngle = Mathf.Clamp(_vTargetAngle, 0f - maxYAngle, maxYAngle);
			_vAngle = _vTargetAngle;
			if (_vAngle > minYAngle)
			{
				_vAngle = minYAngle;
			}
			else if (_vAngle < 0f - maxYAngle)
			{
				_vAngle = 0f - maxYAngle;
			}
		}

		private void PositionFirstPersonCamera()
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			Vector3 noLerp;
			Vector3 val = CalculateCameraPosition(firstPerson: true, out noLerp);
			if (_instantFollow)
			{
				_lastCameraPositionToBlinkFrom = val;
				T.set_localPosition(noLerp);
				Vector3 val2 = noLerp + Quaternion.Euler(_vTargetAngle, _hAngle, 0f) * Vector3.get_back();
				T.LookAt(val2, Vector3.get_up());
				T.set_localPosition(val);
			}
			else if (_progressiveFollow)
			{
				_timer += Time.get_deltaTime();
				Vector3 val3 = Vector3.Lerp(_expectedCameraPosition, val, _timer / _cameraTime);
				Vector3 localPosition = Vector3.Lerp(_lastCameraPositionToBlinkFrom, val3, Quad(_timer / _cameraTime));
				T.set_localPosition(localPosition);
				if (_timer >= _cameraTime)
				{
					_currentSpeed.Set(0f, 0f, 0f);
					_instantFollow = true;
					_progressiveFollow = false;
					_timer = 0f;
				}
			}
		}

		private void PositionThirdPersonCamera()
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			Vector3 noLerp;
			Vector3 val = CalculateCameraPosition(firstPerson: false, out noLerp);
			if (_instantFollow)
			{
				_lastCameraPositionToBlinkFrom = val;
				T.set_localPosition(noLerp);
				Vector3 val2 = CalculateCameraRotation();
				T.LookAt(val2, Vector3.get_up());
				T.set_localPosition(val);
				ForceInsideTerrain(val2);
			}
			else if (_progressiveFollow)
			{
				_timer += Time.get_deltaTime();
				Vector3 val3 = Vector3.Lerp(_expectedCameraPosition, val, _timer / _cameraTime);
				Vector3 localPosition = Vector3.Lerp(_lastCameraPositionToBlinkFrom, val3, Quad(_timer / _cameraTime));
				T.set_localPosition(localPosition);
				if (_timer >= _cameraTime)
				{
					_currentSpeed.Set(0f, 0f, 0f);
					_instantFollow = true;
					_progressiveFollow = false;
					_timer = 0f;
				}
			}
		}

		private static float Quad(float x)
		{
			return x * x;
		}

		private Vector3 CalculateCameraPosition(bool firstPerson, out Vector3 noLerp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_0166: Unknown result type (might be due to invalid IL or missing references)
			noLerp = Vector3.get_zero();
			if (_TrackingCube != null)
			{
				Vector3 val2;
				if (_rigidBody != null)
				{
					Vector3 velocity = _rigidBody.get_velocity();
					float x = velocity.x;
					Vector3 velocity2 = _rigidBody.get_velocity();
					Vector3 val = default(Vector3);
					val._002Ector(x, 0f, velocity2.z);
					float sqrMagnitude = val.get_sqrMagnitude();
					float num = velocityDamper;
					if (sqrMagnitude > velClamp)
					{
						num /= sqrMagnitude / velClamp;
					}
					float num2 = sqrMagnitude - _lastModule;
					float num3 = 0f;
					_lastModule = sqrMagnitude;
					num3 = ((!(num2 < 0f - velocityTolerance)) ? (Time.get_deltaTime() * acceleratingInterpolationTime) : (Time.get_deltaTime() * deceleratingInterpolationTime));
					_currentSpeed = Vector3.Lerp(_currentSpeed, val * num, num3);
					val2 = cameraPivotPoint - _currentSpeed;
				}
				else
				{
					val2 = cameraPivotPoint;
				}
				if (!firstPerson)
				{
					Vector3 val3 = Quaternion.Euler(_vAngle, _hAngle, 0f) * Vector3.get_forward();
					Vector3 val4 = val3 * CameraDistance;
					Vector3 result = val2 + val4;
					noLerp = cameraPivotPoint + val4;
					return result;
				}
				return val2;
			}
			return Vector3.get_zero();
		}

		private Vector3 CalculateCameraRotation()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			if (_TrackingCube != null)
			{
				Vector3 cameraPivotPoint = this.cameraPivotPoint;
				if (_vTargetAngle > _vAngle)
				{
					float num = CameraDistance * Mathf.Sin((_vTargetAngle - _vAngle) * ((float)Math.PI / 180f)) / Mathf.Sin((90f - _vTargetAngle) * ((float)Math.PI / 180f));
					cameraPivotPoint.y += num;
				}
				return CheckTargetCollision(cameraPivotPoint);
			}
			return Vector3.get_zero();
		}

		private Vector3 CheckTargetCollision(Vector3 targetLookAt)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = targetLookAt - (_TrackingCube.get_position() - Vector3.get_up());
			Ray val2 = default(Ray);
			val2._002Ector(_TrackingCube.get_position() - Vector3.get_up(), val);
			RaycastHit val3 = default(RaycastHit);
			if (Physics.SphereCast(val2, cameraCollisionRadius, ref val3, val.get_magnitude(), GameLayers.ENVIRONMENT_LAYER_MASK))
			{
				targetLookAt = val3.get_point() + val3.get_normal() * cameraCollisionRadius;
				_collisionRadiusFactor = Mathf.Clamp(_collisionRadiusFactor + Time.get_deltaTime(), 1f, 3f);
			}
			else
			{
				_collisionRadiusFactor = Mathf.Clamp(_collisionRadiusFactor - Time.get_deltaTime(), 1f, 3f);
			}
			return targetLookAt;
		}

		private void ForceInsideTerrain(Vector3 targetLookAt)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = T.get_position() - targetLookAt;
			Ray val2 = default(Ray);
			val2._002Ector(targetLookAt, val);
			float num = cameraCollisionRadius / _collisionRadiusFactor;
			RaycastHit val3 = default(RaycastHit);
			if (Physics.SphereCast(val2, num, ref val3, val.get_magnitude(), GameLayers.ENVIRONMENT_LAYER_MASK))
			{
				T.set_position(val3.get_point() + val3.get_normal() * num);
			}
		}

		public void SetInstantFollow(bool instantFollow)
		{
			_instantFollow = instantFollow;
		}

		public bool GetInstantFollow()
		{
			return _instantFollow;
		}

		public void SetProgressiveFollow(bool progressiveFollow)
		{
			_progressiveFollow = progressiveFollow;
		}

		public bool GetProgressiveFollow()
		{
			return _progressiveFollow;
		}

		public void SetCameraTime(float cameraTime)
		{
			_cameraTime = cameraTime;
		}

		public float GetCameraTime()
		{
			return _cameraTime;
		}

		public Vector3 GetLastCameraPosition()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _lastCameraPositionToBlinkFrom;
		}

		public void SetExpectedCameraPosition(Vector3 expectedPosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_expectedCameraPosition = expectedPosition;
		}
	}
}
