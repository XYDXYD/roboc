using Services.Web.Photon;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation
{
	internal sealed class PlayerStrafeDirectionManager : IInitialize, IWaitForFrameworkDestruction, IPhysicallyTickable, ITickableBase
	{
		public const float ROTATION_THRESHOLD = 5f;

		private Transform _camTransform;

		private Transform _rbT;

		private bool _strafingEnabled;

		private bool _verticalStrafingEnabled = true;

		private bool _sidewaysDriving = true;

		private bool _trackTurnOnSpot = true;

		private Vector3 _lastCameraDirection = Vector3.get_zero();

		private float _angleToPreviousCamPos;

		private float _angleToStraight;

		private float _angleToHorizontal;

		private float _verticalAngleError;

		private Vector3 _forwardMovementDirection = Vector3.get_zero();

		private Vector3 _rightMovementDirection = Vector3.get_zero();

		private Vector3 _cameraForwardDirection;

		private Vector3 _cameraRightDirection;

		private Vector3 _cameraUpDirection;

		[Inject]
		public MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public Vector3 forwardMovementDirection => _forwardMovementDirection;

		public Vector3 rightMovementDirection => _rightMovementDirection;

		public float angleToStraight => _angleToStraight;

		public float angleToHorizontal => _angleToHorizontal;

		public float verticalAngleError => _verticalAngleError;

		public float angleToPreviousCamPos => _angleToPreviousCamPos;

		public bool strafingEnabled => _strafingEnabled;

		public bool verticalStrafingEnabled => _verticalStrafingEnabled;

		public bool sidewaysDrivingEnabled => _sidewaysDriving;

		public bool tracksTurningOnSpotEnabled => _trackTurnOnSpot;

		public Vector3 cameraForwardDirection => _cameraForwardDirection;

		public Vector3 cameraRightDirection => _cameraRightDirection;

		public Vector3 cameraUpDirection => _cameraUpDirection;

		void IInitialize.OnDependenciesInjected()
		{
			spawnDispatcher.OnPlayerRegistered += OnPlayerRegistered;
			IGetCurrentRobotControlsRequest getCurrentRobotControlsRequest = serviceFactory.Create<IGetCurrentRobotControlsRequest>();
			getCurrentRobotControlsRequest.SetAnswer(new ServiceAnswer<GetRobotControlsResult>(OnControlSettingsLoaded));
			getCurrentRobotControlsRequest.Execute();
		}

		private void OnControlSettingsLoaded(GetRobotControlsResult controls)
		{
			ControlSettings controls2 = controls.controls;
			_strafingEnabled = (controls2.controlType == ControlType.CameraControl);
			_verticalStrafingEnabled = controls2.verticalStrafing;
			_sidewaysDriving = controls2.sidewaysDriving;
			_trackTurnOnSpot = controls2.tracksTurnOnSpot;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			spawnDispatcher.OnPlayerRegistered -= OnPlayerRegistered;
		}

		private void OnPlayerRegistered(SpawnInParametersPlayer spawnInParameters)
		{
			if (spawnInParameters.isMe && _strafingEnabled)
			{
				_rbT = spawnInParameters.preloadedMachine.rbData.get_transform();
				_camTransform = Camera.get_main().get_transform();
				_strafingEnabled = true;
			}
		}

		void IPhysicallyTickable.PhysicsTick(float deltaSec)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (_strafingEnabled && _camTransform != null)
			{
				_cameraForwardDirection = _camTransform.get_forward();
				_cameraRightDirection = _camTransform.get_right();
				_cameraUpDirection = _camTransform.get_up();
				CalculateAngleToPreviousCamPos();
				CalculateMovementDirection();
				CalculateAngleFromStraight();
				_lastCameraDirection = _camTransform.get_forward();
			}
		}

		public void SetControls(ControlType controlSettings)
		{
			_strafingEnabled = (controlSettings == ControlType.CameraControl);
		}

		public bool IsRotating()
		{
			return _angleToPreviousCamPos >= 5f || Mathf.Abs(_angleToStraight) > 5f;
		}

		private void CalculateAngleToPreviousCamPos()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forward = _camTransform.get_forward();
			forward.y = 0f;
			Vector3 lastCameraDirection = _lastCameraDirection;
			lastCameraDirection.y = 0f;
			_angleToPreviousCamPos = Vector3.Angle(forward, lastCameraDirection);
		}

		private void CalculateAngleFromStraight()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forward = _rbT.get_forward();
			_angleToStraight = Vector3.Angle(_forwardMovementDirection, forward);
			float num = Mathf.Sign(Vector3.Dot(_rbT.get_up(), Vector3.Cross(_forwardMovementDirection, forward)));
			_angleToStraight *= num;
			Quaternion rotation = _camTransform.get_rotation();
			Vector3 eulerAngles = rotation.get_eulerAngles();
			_angleToHorizontal = eulerAngles.x;
			if (_angleToHorizontal > 180f)
			{
				_angleToHorizontal -= 360f;
			}
			CalculateVerticalAngleError();
		}

		private void CalculateVerticalAngleError()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forward = _camTransform.get_forward();
			float y = forward.y;
			Vector3 forward2 = _rbT.get_forward();
			_verticalAngleError = y - forward2.y;
		}

		private void CalculateMovementDirection()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forward = _camTransform.get_forward();
			Vector3 up = _rbT.get_up();
			forward = Vector3.ProjectOnPlane(forward, Vector3.get_up());
			Quaternion val = Quaternion.get_identity();
			val.set_eulerAngles(new Vector3(0f, 90f, 0f));
			Vector3 val2 = val * forward;
			float num = Vector3.Angle(up, Vector3.get_up());
			Vector3 val3 = Vector3.Cross(up, Vector3.get_up());
			Vector3 val4 = val2;
			val4 += forward;
			val4.Normalize();
			float num2 = Vector3.Dot(val3.get_normalized(), val2.get_normalized());
			if ((double)val3.get_magnitude() < 0.01)
			{
				num2 = Mathf.Sign(num2);
			}
			num *= 0f - num2;
			val = Quaternion.AngleAxis(num, val2);
			_forwardMovementDirection = val * forward;
			_forwardMovementDirection = Vector3.ProjectOnPlane(_forwardMovementDirection, up);
			_forwardMovementDirection.Normalize();
			val = Quaternion.AngleAxis(90f, up);
			_rightMovementDirection = val * _forwardMovementDirection;
		}

		public bool IsAngleToStraightGreaterThanThreshold(float multiplier = 1f)
		{
			return Mathf.Abs(_angleToStraight) > 5f * multiplier;
		}

		public bool IsAngleGreaterThanThreshold(float angle, float multiplier = 1f)
		{
			return Mathf.Abs(angle) > 5f * multiplier;
		}

		public bool IsAngleToPrevCamPosGreaterThanThreshold()
		{
			return Mathf.Abs(_angleToPreviousCamPos) > 5f;
		}

		public float GetAngleToThresholdRatio()
		{
			return Mathf.Abs(_angleToStraight) / 5f;
		}
	}
}
