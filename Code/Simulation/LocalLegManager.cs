using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class LocalLegManager : LegManager
	{
		private const float MAX_SPEED_THRESHOLD = 0.25f;

		private const float MAX_ANGULAR_VELOCITY = 3f;

		private float _inAirRotationSpeed = 60f;

		private float _inAirRotationAccn = 90f;

		private float _inAirRotationFullDuration = 1f;

		private float _inAirRotationFadeDuration = 1f;

		private float _jumpDuration = 0.6f;

		private float _wrongDirectionStandingForceScale = 0.5f;

		private IMachineControl _inputWrapper;

		private Vector4 _inputVector;

		private float _timeLastGrounded;

		private float _mass;

		private float _jumpTimeRemaining;

		private bool _jumpForcePending;

		private Rigidbody _rb;

		private bool _waitedInitialFrame;

		private bool _functionalsEnabled = true;

		private bool _legacyControls;

		private PlayerStrafeDirectionManager _strafeDirectionManager;

		private bool _isRotating;

		public LocalLegManager(Rigidbody rb, IMachineControl inputWrapper, PlayerStrafeDirectionManager playerStrafeDirectionManager)
		{
			_rb = rb;
			_inputWrapper = inputWrapper;
			_legGraphics = new LegGraphicsManager(_rb.get_gameObject());
			_raycastMask = GameLayers.INTERACTIVE_LAYER_MASK;
			_strafeDirectionManager = playerStrafeDirectionManager;
			_legacyControls = !playerStrafeDirectionManager.strafingEnabled;
		}

		public LocalLegManager(Rigidbody rb, IMachineControl inputWrapper)
		{
			_rb = rb;
			_inputWrapper = inputWrapper;
			_legGraphics = new LegGraphicsManager(_rb.get_gameObject());
			_raycastMask = GameLayers.INTERACTIVE_AI_LAYER_MASK;
			_legacyControls = true;
		}

		public override void RegisterLeg(CubeLeg leg)
		{
			base.RegisterLeg(leg);
			UpdateLegMass();
			if (!_functionalsEnabled)
			{
				PauseLeg(leg.legData);
			}
		}

		public override void UnregisterLeg(CubeLeg leg)
		{
			base.UnregisterLeg(leg);
			UpdateLegMass();
		}

		public override void Tick(float deltaTime)
		{
			if (_legs.Count > 0)
			{
				if (_functionalsEnabled)
				{
					ProcessInput();
				}
				_legGraphics.Tick(deltaTime);
			}
		}

		public override void PhysicsTick(float deltaTime)
		{
			if (_legs.Count <= 0)
			{
				return;
			}
			if (!_waitedInitialFrame)
			{
				_waitedInitialFrame = true;
				return;
			}
			ProcessLegMass();
			ProcessJumpForce(deltaTime);
			if (_functionalsEnabled)
			{
				ProcessInAirRotation(deltaTime);
			}
			if (!_legacyControls)
			{
				_isRotating = _strafeDirectionManager.IsRotating();
			}
			for (int i = 0; i < _legs.Count; i++)
			{
				ProcessLeg(_legs[i], deltaTime);
			}
		}

		public override void SetJumpState()
		{
			_justJumped = true;
			RobotJumped();
		}

		internal override void DisableLegs()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			_functionalsEnabled = false;
			_inputVector = Vector4.get_zero();
			_distToGround = float.MaxValue;
			for (int i = 0; i < _legs.Count; i++)
			{
				PauseLeg(_legs[i].legData);
			}
		}

		private void PauseLeg(LegData legData)
		{
			legData.legGrounded = false;
			legData.jumping = true;
		}

		internal override void EnableLegs()
		{
			_functionalsEnabled = true;
		}

		private void ProcessInput()
		{
			ConvertCubeInputToVector3();
			DecideIfJumping();
		}

		private void ProcessLeg(CubeLeg leg, float deltaTime)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			leg.UpdateCachedValues();
			leg.UpdateForcePoint(_rb.get_worldCenterOfMass());
			if (!leg.legData.initialised)
			{
				InitialiseLeg(leg);
			}
			DecideIfCrouching(leg);
			if (_functionalsEnabled)
			{
				UpdateTargetGroundedPosition(leg, deltaTime);
			}
			ProcessCurrentVelocity(leg, deltaTime);
			ApplyForces(leg);
		}

		private void InitialiseLeg(CubeLeg leg)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			leg.legData.initialised = true;
			leg.legData.currentVelocity = Vector3.get_zero();
			leg.legData.currentSpeed = 0f;
			leg.legData.targetLegPosition = (leg.legData.lastPosition = leg.exactFootPosition);
		}

		private void ApplyForces(CubeLeg leg)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			ProcessStoppedState(leg);
			if (!leg.legData.jumping && !_justJumped)
			{
				Vector3 val = Vector3.get_zero();
				if (leg.legData.legGrounded)
				{
					val += ApplyStandingForce(leg);
					val += ApplyStandingDamping(leg);
					val += ApplyLateralForce(leg, _inputVector);
					val += ApplyStoppedForce(leg);
					val += ApplySwaggerForce(leg);
				}
				_rb.AddForceAtPosition(val, leg.forcePoint);
			}
		}

		private void ConvertCubeInputToVector3()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (_inputWrapper != null)
			{
				_inputVector = Vector4.get_zero();
				if (_inputWrapper.horizontalAxis > 0f)
				{
					_inputVector.x = 1f;
				}
				else if (_inputWrapper.horizontalAxis < 0f)
				{
					_inputVector.x = -1f;
				}
				else
				{
					_inputVector.x = 0f;
				}
				if (_inputWrapper.moveUpwards)
				{
					_inputVector.y = 1f;
				}
				else if (_inputWrapper.moveDown)
				{
					_inputVector.y = -1f;
				}
				else
				{
					_inputVector.y = 0f;
				}
				if (_inputWrapper.forwardAxis > 0f)
				{
					_inputVector.z = 1f;
				}
				else if (_inputWrapper.forwardAxis < 0f)
				{
					_inputVector.z = -1f;
				}
				else
				{
					_inputVector.z = 0f;
				}
				if (_inputWrapper.strafeLeft)
				{
					_inputVector.w = 1f;
				}
				else if (_inputWrapper.strafeRight)
				{
					_inputVector.w = -1f;
				}
				else
				{
					_inputVector.w = 0f;
				}
				if (_legacyControls && _inputVector.z < -0.5f)
				{
					_inputVector.x = 0f - _inputVector.x;
				}
			}
		}

		private void DecideIfJumping()
		{
			_jumpForcePending = false;
			if (_inputVector.y > 0f)
			{
				if (_justJumped)
				{
					return;
				}
				for (int i = 0; i < _legs.Count; i++)
				{
					if (_legs[i].legData.jumping)
					{
						return;
					}
				}
				_jumpForcePending = true;
			}
			else
			{
				_justJumped = false;
			}
		}

		private void DecideIfCrouching(CubeLeg leg)
		{
			if (_inputVector.y < 0f)
			{
				leg.legData.targetHeight = leg.movement.idealCrouchingHeight;
			}
			else
			{
				leg.legData.targetHeight = leg.movement.idealHeight;
			}
		}

		private void ProcessStoppedState(CubeLeg leg)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			if ((_legacyControls || leg.legData.jumping || _justJumped || !leg.legData.legGrounded) && !_legacyControls)
			{
				return;
			}
			Vector4 inputVector = _inputVector;
			inputVector.y = 0f;
			if (!leg.legData.stopped)
			{
				if (inputVector.get_sqrMagnitude() < 0.1f && !_isRotating)
				{
					leg.legData.stopped = true;
					leg.legData.stoppedPosition = leg.legData.targetLegPosition;
					leg.legData.stoppedRbPosition = _rb.get_position();
				}
			}
			else if (inputVector.get_sqrMagnitude() >= 0.1f || _isRotating)
			{
				leg.legData.stopped = false;
			}
		}

		private Vector3 ApplyStandingForce(CubeLeg leg)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = leg.movement.maxUpwardsForce * GetClingDirection(leg);
			float num = Vector3.Dot(Vector3.get_up(), val.get_normalized());
			num = (num + 1f) * 0.5f;
			if (num < 0f)
			{
				val -= val * _wrongDirectionStandingForceScale * Mathf.Abs(num);
			}
			return val;
		}

		private Vector3 ApplyStandingDamping(CubeLeg leg)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Dot(leg.legData.currentVelocity, leg.up);
			float num2 = 1f - 1f / leg.legData.targetHeight * Mathf.Clamp(Mathf.Abs(_distToGround - leg.legData.targetHeight), 0f, leg.legData.targetHeight);
			float num3 = Mathf.Clamp(leg.movement.upwardsDampingForce * num, 0f - leg.movement.maxDampingForce, leg.movement.maxDampingForce);
			return leg.down * num3 * num2;
		}

		private Vector3 GetClingDirection(CubeLeg leg)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			if (_distToGround < leg.legData.targetHeight - leg.movement.idealHeightRange)
			{
				return leg.up;
			}
			if (_distToGround < leg.legData.targetHeight)
			{
				return leg.up * (leg.legData.targetHeight - _distToGround) / leg.movement.idealHeightRange;
			}
			if (_distToGround > leg.legData.targetHeight + leg.movement.idealHeightRange)
			{
				return leg.down;
			}
			if (_distToGround > leg.legData.targetHeight)
			{
				return leg.down * (_distToGround - leg.legData.targetHeight) / leg.movement.idealHeightRange;
			}
			return Vector3.get_zero();
		}

		private Vector3 ApplyLateralForce(CubeLeg leg, Vector4 inputVector)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			if (_legacyControls)
			{
				return ApplyLegacyLateralForce(leg, inputVector);
			}
			return ApplyStrafingLateralForce(leg, inputVector);
		}

		private Vector3 ApplyLegacyLateralForce(CubeLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			CalculateLegacyMovementDirection(leg, out Vector3 forwardDirection, out Vector3 leftDirection);
			if (!leg.legData.stopped)
			{
				val += ApplyLegacyLateralMovementForces(leg, inputVector, forwardDirection, leftDirection);
			}
			val += ApplyLegacyTurningForce(leg, inputVector, leftDirection);
			val += ApplyTurnDampingForce(leg, inputVector, leftDirection);
			Vector3 val2 = val.get_normalized() - _rb.get_transform().get_up() * 0.3f;
			val = val2.get_normalized() * val.get_magnitude();
			return val;
		}

		private Vector3 ApplyTurnDampingForce(CubeLeg leg, Vector4 inputVector, Vector3 leftDirection)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			if (Mathf.Abs(inputVector.x) <= 0.1f)
			{
				float num = Vector3.Dot(_rb.get_angularVelocity(), _rb.get_transform().get_up());
				float num2 = num / 3f;
				if (num2 > 0.01f)
				{
					result = ((!leg.legData.xInputFlipped) ? (ApplyLateralForceVector(leg, -leftDirection, leg.movement.maxTurningForce) * (0f - num2)) : (ApplyLateralForceVector(leg, leftDirection, leg.movement.maxTurningForce) * (0f - num2)));
				}
				else if (num2 < -0.01f)
				{
					result = ((!leg.legData.xInputFlipped) ? (ApplyLateralForceVector(leg, leftDirection, leg.movement.maxTurningForce) * num2) : (ApplyLateralForceVector(leg, -leftDirection, leg.movement.maxTurningForce) * num2));
				}
			}
			return result;
		}

		private void CalculateLegacyMovementDirection(CubeLeg leg, out Vector3 forwardDirection, out Vector3 leftDirection)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			forwardDirection = (leftDirection = Vector3.get_zero());
			switch (leg.legData.facingDirection)
			{
			case CubeFace.Back:
				forwardDirection = leg.back;
				leftDirection = leg.right;
				break;
			case CubeFace.Front:
				forwardDirection = leg.forward;
				leftDirection = leg.left;
				break;
			case CubeFace.Left:
				forwardDirection = leg.right;
				leftDirection = leg.forward;
				break;
			case CubeFace.Right:
				forwardDirection = leg.left;
				leftDirection = leg.back;
				break;
			}
		}

		private Vector3 ApplyLegacyLateralMovementForces(CubeLeg leg, Vector4 inputVector, Vector3 forwardDirection, Vector3 leftDirection)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0170: Unknown result type (might be due to invalid IL or missing references)
			//IL_0175: Unknown result type (might be due to invalid IL or missing references)
			//IL_0176: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			val = ((inputVector.z > 0.1f) ? (val + ApplyLateralForceVector(leg, forwardDirection, leg.movement.maxLateralForce)) : ((!(inputVector.z < -0.1f)) ? (val + ApplyLateralDampingVector(leg, forwardDirection, leg.movement.lateralDampForce)) : (val + ApplyLateralForceVector(leg, -forwardDirection, leg.movement.maxLateralForce))));
			if (inputVector.w > 0.1f)
			{
				val += ApplyLateralForceVector(leg, leftDirection, leg.movement.maxLateralForce);
			}
			else if (inputVector.w < -0.1f)
			{
				val += ApplyLateralForceVector(leg, -leftDirection, leg.movement.maxLateralForce);
			}
			else if (inputVector.x < 0.1f && inputVector.x > -0.1f)
			{
				val += ApplyLateralDampingVector(leg, leftDirection, leg.movement.lateralDampForce);
			}
			if (val.get_sqrMagnitude() > leg.movement.maxLateralForce * leg.movement.maxLateralForce)
			{
				val = val.get_normalized() * leg.movement.maxLateralForce;
			}
			return val;
		}

		private Vector3 ApplyLegacyTurningForce(CubeLeg leg, Vector4 inputVector, Vector3 leftDirection)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			float angularY = Vector3.Dot(_rb.get_angularVelocity(), _rb.get_transform().get_up());
			float turningRatio = GetTurningRatio(angularY, inputVector.x, 3f);
			if (inputVector.x > 0.1f)
			{
				val = ((!leg.legData.xInputFlipped) ? (val + ApplyLateralForceVector(leg, -leftDirection, leg.movement.maxTurningForce)) : (val + ApplyLateralForceVector(leg, leftDirection, leg.movement.maxTurningForce)));
			}
			else if (inputVector.x < -0.1f)
			{
				val = ((!leg.legData.xInputFlipped) ? (val + ApplyLateralForceVector(leg, leftDirection, leg.movement.maxTurningForce)) : (val + ApplyLateralForceVector(leg, -leftDirection, leg.movement.maxTurningForce)));
			}
			return val * turningRatio;
		}

		private float GetTurningRatio(float angularY, float turningInput, float maxAngularVelocity)
		{
			if (Mathf.Abs(angularY) < maxAngularVelocity || angularY * turningInput < 0f)
			{
				return 1f;
			}
			return 1f - Mathf.Clamp01((Mathf.Abs(angularY) - maxAngularVelocity) / (maxAngularVelocity * 0.25f));
		}

		private Vector3 ApplyStrafingLateralForce(CubeLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			val += ApplyStrafingForwardForce(leg, inputVector);
			val += ApplyStrafingForwardDampingForce(leg, inputVector);
			val += ApplyStrafingTurningForce(leg, inputVector);
			Vector3 val2 = val.get_normalized() - _rb.get_transform().get_up() * 0.3f;
			val = val2.get_normalized() * val.get_magnitude();
			return val;
		}

		private Vector3 ApplyStrafingForwardForce(CubeLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			Vector3 forwardMovementDirection = _strafeDirectionManager.forwardMovementDirection;
			Vector3 rightMovementDirection = _strafeDirectionManager.rightMovementDirection;
			if (inputVector.z > 0.1f)
			{
				val += ApplyLateralForceVector(leg, forwardMovementDirection, leg.movement.maxLateralForce);
			}
			else if (inputVector.z < -0.1f)
			{
				val += ApplyLateralForceVector(leg, -forwardMovementDirection, leg.movement.maxLateralForce);
			}
			if (inputVector.x > 0.1f)
			{
				val += ApplyLateralForceVector(leg, rightMovementDirection, leg.movement.maxLateralForce);
			}
			else if (inputVector.x < -0.1f)
			{
				val += ApplyLateralForceVector(leg, -rightMovementDirection, leg.movement.maxLateralForce);
			}
			return val;
		}

		private Vector3 ApplyStrafingForwardDampingForce(CubeLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			if (inputVector.z < 0.1f && inputVector.z > -0.1f && !_isRotating)
			{
				val = ((!leg.legData.xIsSideways) ? (val + ApplyLateralDampingVector(leg, leg.left, leg.movement.lateralDampForce)) : (val + ApplyLateralDampingVector(leg, leg.forward, leg.movement.lateralDampForce)));
			}
			if (inputVector.x < 0.1f && inputVector.x > -0.1f && !_isRotating)
			{
				val = ((!leg.legData.xIsSideways) ? (val + ApplyLateralDampingVector(leg, leg.forward, leg.movement.lateralDampForce)) : (val + ApplyLateralDampingVector(leg, leg.left, leg.movement.lateralDampForce)));
			}
			return val;
		}

		private Vector3 ApplyStrafingTurningForce(CubeLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			if (_strafeDirectionManager.IsAngleToStraightGreaterThanThreshold())
			{
				CalculateLegacyMovementDirection(leg, out Vector3 _, out Vector3 leftDirection);
				float angleToStraight = _strafeDirectionManager.angleToStraight;
				float num = Mathf.Clamp01(Mathf.Abs(angleToStraight) / 90f);
				float angularY = Vector3.Dot(_rb.get_angularVelocity(), _rb.get_transform().get_up());
				num *= GetTurningRatio(angularY, 0f - angleToStraight, 3f);
				if (angleToStraight < 0f)
				{
					val = ((!leg.legData.xInputFlipped) ? (val + ApplyLateralForceVector(leg, -leftDirection, leg.movement.maxTurningForce)) : (val + ApplyLateralForceVector(leg, leftDirection, leg.movement.maxTurningForce)));
				}
				else if (angleToStraight > 0f)
				{
					val = ((!leg.legData.xInputFlipped) ? (val + ApplyLateralForceVector(leg, leftDirection, leg.movement.maxTurningForce)) : (val + ApplyLateralForceVector(leg, -leftDirection, leg.movement.maxTurningForce)));
				}
				if ((double)Mathf.Abs(inputVector.x) < 0.1)
				{
					val *= num;
				}
			}
			return val;
		}

		private Vector3 ApplyLateralForceVector(CubeLeg leg, Vector3 vector, float maxForce)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return vector * maxForce;
		}

		private Vector3 ApplyLateralDampingVector(CubeLeg leg, Vector3 direction, float damping, Color color = default(Color))
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Dot(leg.legData.currentVelocity, direction);
			float num2 = CalculateLateralDampingMultiplier(leg.maxSpeed, num);
			float num3 = Mathf.Clamp(damping * (num * num2), 0f - leg.movement.maxDampingForce, leg.movement.maxDampingForce);
			return -direction * num3;
		}

		private float CalculateLateralDampingMultiplier(float maxLateralSpeed, float currentSpeed)
		{
			float num = 0.25f;
			if (!_legacyControls)
			{
				num = 0.1f;
			}
			float num2 = maxLateralSpeed * 0.75f;
			float num3 = maxLateralSpeed * num;
			float result = 1f;
			float num4 = Mathf.Abs(currentSpeed);
			if (num4 < num3)
			{
				result = num;
			}
			else if (num4 < num2)
			{
				result = num4 / num2;
			}
			return result;
		}

		private Vector3 ApplyLegacyStoppedForce(CubeLeg leg)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			if (leg.legData.stopped)
			{
				Vector3 val = leg.legData.stoppedPosition - leg.position;
				float num = Vector3.Dot(leg.right, val);
				float num2 = num / leg.validLegRadius;
				float num3 = Vector3.Dot(leg.forward, val) - leg.footOffsetToLegBase;
				float num4 = num3 / leg.validLegRadius;
				result = (leg.right * num2 + leg.forward * num4) * leg.movement.maxStoppedForce;
			}
			return result;
		}

		private Vector3 ApplyStoppedForce(CubeLeg leg)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			Vector3 zero = Vector3.get_zero();
			if (leg.legData.stopped)
			{
				Vector3 val = _rb.get_velocity() + Physics.get_gravity() * Time.get_fixedDeltaTime();
				Vector3 velocity = _rb.get_velocity();
				Vector3 normalized = velocity.get_normalized();
				float num = Vector3.Dot(normalized, _rb.get_transform().get_up());
				if ((double)Mathf.Abs(num) < 0.8)
				{
					float magnitude = val.get_magnitude();
					float num2 = leg.movement.maxStoppedForce * Time.get_fixedDeltaTime() * (float)_legs.Count;
					float num3 = Mathf.Max(magnitude - num2, 0f);
					float num4 = (num3 - magnitude) / (float)_legs.Count;
					_rb.AddForce(num4 * normalized, 2);
				}
			}
			return zero;
		}

		private Vector3 ApplyStrafingStoppedForce(CubeLeg leg)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			if (leg.legData.stopped)
			{
				Vector3 forwardMovementDirection = _strafeDirectionManager.forwardMovementDirection;
				Vector3 rightMovementDirection = _strafeDirectionManager.rightMovementDirection;
				ApplyVelocitySlowingForce(leg);
				float num = Vector3.Dot(rightMovementDirection, leg.legData.stoppedRbPosition - _rb.get_position());
				float num2 = Vector3.Dot(forwardMovementDirection, leg.legData.stoppedRbPosition - _rb.get_position());
				float num3 = num / leg.validLegRadius;
				float num4 = num2 / leg.validLegRadius;
				if (Mathf.Abs(num3) > 1f || Mathf.Abs(num4) > 1f)
				{
					leg.legData.stoppedRbPosition = _rb.get_position();
					return result;
				}
				if (Mathf.Abs(num) < leg.validLegRadius)
				{
					num3 *= num3;
				}
				if (Mathf.Abs(num2) < leg.validLegRadius)
				{
					num4 *= num4;
				}
				result = (rightMovementDirection * num3 + forwardMovementDirection * num4) * leg.movement.maxNewStoppedForce;
			}
			return result;
		}

		private void ApplyVelocitySlowingForce(CubeLeg leg)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forwardMovementDirection = _strafeDirectionManager.forwardMovementDirection;
			Vector3 rightMovementDirection = _strafeDirectionManager.rightMovementDirection;
			Vector3 velocity = _rb.get_velocity();
			float num = Vector3.Dot(rightMovementDirection, velocity);
			float num2 = Vector3.Dot(forwardMovementDirection, velocity);
			Vector3 val = rightMovementDirection * num + forwardMovementDirection * num2;
			val *= 0f - leg.stoppedVelocityScale;
			_rb.AddForce(val, 2);
		}

		private Vector3 ApplySwaggerForce(CubeLeg leg)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			if (!leg.legData.stopped && !leg.legGraphics.isAnimating)
			{
				result = leg.T.get_rotation() * leg.swaggerDirection.get_normalized() * leg.movement.swaggerForce;
			}
			return result;
		}

		private void ProcessJumpForce(float deltaTime)
		{
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			bool flag2 = true;
			for (int i = 0; i < _legs.Count; i++)
			{
				flag |= _legs[i].legData.jumping;
				flag2 &= _legs[i].legData.canJump;
			}
			if (!flag)
			{
				if (_jumpForcePending && flag2)
				{
					_jumpForcePending = false;
					_justJumped = true;
					RobotJumped();
					float num = 0f;
					for (int j = 0; j < _legs.Count; j++)
					{
						num += _legs[j].movement.jumpHeight;
					}
					float num2 = num / (float)_legs.Count;
					float num3 = 2f * num2;
					Vector3 gravity = Physics.get_gravity();
					float num4 = Mathf.Sqrt(num3 * gravity.get_magnitude());
					_rb.AddForce(_rb.get_transform().get_up() * num4, 2);
				}
			}
			else
			{
				if (_justJumped)
				{
					return;
				}
				_jumpTimeRemaining -= deltaTime;
				if (_jumpTimeRemaining <= 0f)
				{
					for (int k = 0; k < _legs.Count; k++)
					{
						_legs[k].legData.jumping = false;
					}
				}
			}
		}

		private void RobotJumped()
		{
			_jumpTimeRemaining = _jumpDuration;
			for (int i = 0; i < _legs.Count; i++)
			{
				_legs[i].legData.jumping = true;
			}
		}

		private void ProcessInAirRotation(float deltaTime)
		{
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0231: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			for (int i = 0; i < _legs.Count; i++)
			{
				if (_legs[i].legData.legGrounded)
				{
					flag = true;
					break;
				}
			}
			if (!flag && _rb != null)
			{
				float num = Time.get_realtimeSinceStartup() - _timeLastGrounded;
				if (!(num < _inAirRotationFullDuration + _inAirRotationFadeDuration))
				{
					return;
				}
				Vector3 zero = Vector3.get_zero();
				Quaternion localRotation = _rb.get_transform().get_localRotation();
				Vector3 eulerAngles = localRotation.get_eulerAngles();
				eulerAngles.y = 0f;
				if (eulerAngles.x > 180f)
				{
					eulerAngles.x -= 360f;
				}
				if (eulerAngles.z > 180f)
				{
					eulerAngles.z -= 360f;
				}
				Vector3 val = Quaternion.Inverse(_rb.get_transform().get_rotation()) * (_rb.get_angularVelocity() * 57.29578f);
				val.y = 0f;
				Vector3 val2 = -eulerAngles;
				Vector3 zero2 = Vector3.get_zero();
				float num2 = _inAirRotationAccn;
				if (num > _inAirRotationFullDuration)
				{
					num2 *= 1f - Mathf.Clamp01((num - _inAirRotationFullDuration) / _inAirRotationFadeDuration);
				}
				float num3 = _inAirRotationSpeed / num2;
				Vector3 val3 = val;
				for (int j = 0; j < 3; j++)
				{
					if (j != 1)
					{
						zero2.set_Item(j, num3);
						int num4;
						val3.set_Item(num4 = j, val3.get_Item(num4) * zero2.get_Item(j));
						if (Mathf.Abs(val3.get_Item(j)) > Mathf.Abs(val2.get_Item(j)))
						{
							zero.set_Item(j, (0f - num2) * deltaTime * Mathf.Sign(val3.get_Item(j)));
						}
						else
						{
							zero.set_Item(j, num2 * deltaTime * Mathf.Sign(val2.get_Item(j)));
						}
					}
				}
				_rb.AddRelativeTorque(zero * ((float)Math.PI / 180f), 2);
			}
			else
			{
				_timeLastGrounded = Time.get_realtimeSinceStartup();
			}
		}

		private void ProcessLegMass()
		{
			if (Mathf.RoundToInt(_rb.get_mass()) != Mathf.RoundToInt(_mass))
			{
				UpdateLegMass();
			}
		}

		private void UpdateLegMass()
		{
			if (_rb != null)
			{
				_mass = _rb.get_mass();
				float massPerLeg = _mass / (float)_legs.Count;
				for (int i = 0; i < _legs.Count; i++)
				{
					CubeLeg cubeLeg = _legs[i];
					cubeLeg.legData.massPerLeg = massPerLeg;
				}
			}
		}
	}
}
