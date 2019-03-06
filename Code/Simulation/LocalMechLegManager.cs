using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class LocalMechLegManager : MechLegManager
	{
		private const float NEW_FORCE_POS_FORCE_RATIO = 0.85f;

		private const float LEG_FORCE_POS_FORCE_RATIO = 0.15f;

		private const float ORIENTATION_DAMPING = 0.15f;

		private const int VERTICAL_JUMP_ANGLE = 70;

		private const float SMALL_JUMP_SCALE = 0.5f;

		private const float LEG_SPECIFIC_STANDING_FORCE_RACTIO = 0.5f;

		private float _inAirRotationSpeed = 60f;

		private float _inAirRotationAccn = 90f;

		private float _inAirRotationFullDuration = 1f;

		private float _inAirRotationFadeDuration = 1f;

		private IMachineControl _inputWrapper;

		private Vector4 _inputVector;

		private float _timeLastGrounded;

		private bool _fullJumpForcePending;

		private bool _halfJumpForcePending;

		private float _jumpPressedTime;

		private float _inverseDeltaTime = 1f;

		private Transform _rbT;

		private bool _updateLegs = true;

		private bool _functionalsEnabled = true;

		private bool _waitedInitialFrame;

		private bool _legacyControls;

		private PlayerStrafeDirectionManager _strafeDirectionManager;

		private bool _isRotating;

		public LocalMechLegManager(Rigidbody rb, NetworkMachineManager machineManager, IMachineControl inputWrapper, PlayerStrafeDirectionManager playerStrafeDirectionManager)
			: base(rb, machineManager, localPlayer: true)
		{
			_rbT = _rb.get_transform();
			_inputWrapper = inputWrapper;
			_raycastMask = GameLayers.INTERACTIVE_LAYER_MASK;
			_strafeDirectionManager = playerStrafeDirectionManager;
			_legacyControls = !playerStrafeDirectionManager.strafingEnabled;
		}

		public LocalMechLegManager(Rigidbody rb, NetworkMachineManager machineManager, IMachineControl inputWrapper)
			: base(rb, machineManager, localPlayer: false)
		{
			_rbT = _rb.get_transform();
			_inputWrapper = inputWrapper;
			_raycastMask = GameLayers.INTERACTIVE_AI_LAYER_MASK;
			_legacyControls = true;
		}

		public override void RegisterLeg(CubeMechLeg leg)
		{
			base.RegisterLeg(leg);
			UpdateLegMass();
			SetColliderEnabled(leg, 2, enabled: false);
			if (!_functionalsEnabled)
			{
				PauseLeg(leg);
			}
		}

		public override void UnregisterLeg(CubeMechLeg leg)
		{
			base.UnregisterLeg(leg);
			UpdateLegMass();
		}

		public override void Tick(float deltaTime)
		{
			if (_updateLegs && _legs.Count > 0)
			{
				if (_functionalsEnabled)
				{
					ProcessInput();
				}
				UpdateGraphics(deltaTime);
			}
		}

		public override void PhysicsTick(float deltaTime)
		{
			if (!_updateLegs || _legs.Count <= 0)
			{
				return;
			}
			if (!_waitedInitialFrame)
			{
				_waitedInitialFrame = true;
				return;
			}
			_inverseDeltaTime = 1f / deltaTime;
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
			int num = 0;
			for (int i = 0; i < _legs.Count; i++)
			{
				if (_legs[i].isValid)
				{
					num++;
				}
			}
			for (int j = 0; j < _legs.Count; j++)
			{
				UpdateMinItemsModifier(_legs[j], num);
				ProcessLeg(_legs[j], deltaTime);
			}
			UpdateColliders();
		}

		private void UpdateMinItemsModifier(CubeMechLeg leg, int count)
		{
			leg.minItemsModifier = ((count >= leg.minRequiredItems) ? 1f : leg.minItemsModifierVal);
		}

		internal override void PauseLegs(bool pause)
		{
			_updateLegs = !pause;
		}

		public override void SetJumpState()
		{
			_justJumped = true;
			RobotJumped(longJump: false);
		}

		private void UpdateColliders()
		{
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeMechLeg cubeMechLeg = _legs[i];
				if (!cubeMechLeg.isHidden && !cubeMechLeg.legData.legGrounded)
				{
					SetColliderEnabled(cubeMechLeg, 1, enabled: true);
					SetColliderEnabled(cubeMechLeg, 0, enabled: false);
				}
				else
				{
					SetColliderEnabled(cubeMechLeg, 1, enabled: false);
					SetColliderEnabled(cubeMechLeg, 0, enabled: true);
				}
			}
		}

		private void SetColliderEnabled(CubeMechLeg leg, int colliderIndex, bool enabled)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			if (leg.colliders[colliderIndex].get_enabled() != enabled)
			{
				Vector3 centerOfMass = _rb.get_centerOfMass();
				Vector3 inertiaTensor = _rb.get_inertiaTensor();
				Quaternion inertiaTensorRotation = _rb.get_inertiaTensorRotation();
				leg.colliders[colliderIndex].set_enabled(enabled);
				_rb.set_centerOfMass(centerOfMass);
				_rb.set_inertiaTensor(inertiaTensor);
				_rb.set_inertiaTensorRotation(inertiaTensorRotation);
			}
		}

		internal override void EnableMechLegs()
		{
			_functionalsEnabled = true;
			for (int i = 0; i < _legs.Count; i++)
			{
				UnPauseLeg(i);
			}
		}

		internal override void DisableMechLegs()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			_functionalsEnabled = false;
			_inputVector = Vector4.get_zero();
			for (int i = 0; i < _legs.Count; i++)
			{
				PauseLeg(_legs[i]);
			}
		}

		private void PauseLeg(CubeMechLeg leg)
		{
			MechLegData legData = leg.legData;
			SetColliderEnabled(leg, 0, enabled: false);
			SetColliderEnabled(leg, 1, enabled: false);
			SetColliderEnabled(leg, 2, enabled: true);
			legData.legGrounded = false;
			legData.jumping = true;
			legData.vertDistToGround = float.MaxValue;
		}

		private void UnPauseLeg(int i)
		{
			CubeMechLeg leg = _legs[i];
			SetColliderEnabled(leg, 0, enabled: true);
			SetColliderEnabled(leg, 1, enabled: false);
			SetColliderEnabled(leg, 2, enabled: false);
		}

		private void ProcessInput()
		{
			ConvertCubeInputToVector4();
			DecideIfJumping();
		}

		private void ProcessLeg(CubeMechLeg leg, float deltaTime)
		{
			leg.UpdateCachedValues();
			if (!leg.legData.initialised)
			{
				InitialiseLeg(leg);
			}
			DecideIfCrouching(leg);  // ÉèlegData.targetHeight
			if (_functionalsEnabled)
			{
				UpdateGroundedOnItself(leg);
				if (!leg.legData.groundedOnItself)
				{
					UpdateVerticalGroundPosition(leg, deltaTime);
					UpdateTargetGroundedPosition(leg, deltaTime);
				}
				CheckIsDescending(leg);
				ProcessCurrentVelocity(leg, deltaTime);
				CheckIsFlying(leg);
				ApplyForces(leg, deltaTime);
			}
		}

		private void InitialiseLeg(CubeMechLeg leg)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			legData.initialised = true;
			legData.currentVelocity = Vector3.get_zero();
			legData.previousVelocity = Vector3.get_zero();
			legData.currentSpeed = 0f;
			legData.targetLegPosition = (legData.lastPosition = leg.footIKTransform.get_position());
		}

		private void CheckIsFlying(CubeMechLeg leg)
		{
			if (leg.legData.jumping && !leg.legData.legGrounded && !_justJumped && leg.legData.currentVelocity.y > leg.legData.previousVelocity.y)
			{
				leg.legData.jumping = false;
			}
		}

		private float CalculateTargetHeight(CubeMechLeg leg)
		{
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			float num = legData.targetHeight;
			if (legData.legSliding)
			{
				num = leg.movement.idealSlideHeight;
			}
			else
			{
				for (int i = 0; i < _legs.Count; i++)
				{
					CubeMechLeg cubeMechLeg = _legs[i];
					if (cubeMechLeg.legGraphics.animationProgress > cubeMechLeg.startOfDropTime && cubeMechLeg.legGraphics.isAnimating)
					{
						float num2 = Vector3.Dot(legData.currentVelocity, leg.right + leg.forward);
						if (Mathf.Abs(num2) > 0.1f)
						{
							float num3 = Mathf.Abs(num2) / leg.maxSpeed;
							num -= leg.movement.maxBobHeight * num3;
							break;
						}
					}
				}
			}
			return num;
		}

		private void ApplyForces(CubeMechLeg leg, float deltaTime)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			if (!leg.legData.groundedOnItself)
			{
				ProcessStoppedState(leg);
				Vector4 inputVector = GetInputVector(leg);
				if (!leg.legData.jumping && !_justJumped)
				{
					ApplyGroundedForces(leg, inputVector, deltaTime);
				}
				else
				{
					ApplyInAirForces(leg, inputVector, deltaTime);
				}
			}
		}

		private void ApplyGroundedForces(CubeMechLeg leg, Vector4 inputVector, float deltaTime)
		{
			Vector3 zero = Vector3.get_zero();
			Vector3 zero2 = Vector3.get_zero();
			MechLegData legData = leg.legData;
			if (legData.legGrounded)
			{
				Vector3 val = CalculateForcePoint(leg);
				float targetHeight = CalculateTargetHeight(leg);
				Vector3 standingForceDirectionAndMagnitude = GetStandingForceDirectionAndMagnitude(leg, targetHeight);
				zero += CalculateStandingForce(leg, standingForceDirectionAndMagnitude);
				zero += CalculateStandingDamping(leg, targetHeight);
				Vector3 val2 = CalculateLegSpecificStandingForce(leg, standingForceDirectionAndMagnitude);
				zero += CalculateTooLowForce(leg);
				if (!legData.legSliding && !legData.legShouldSlide)
				{
					zero += CalculateLateralForce(leg, inputVector);
					zero += CalculateLateralDampingForce(leg, inputVector);
					zero += CalculateStoppedForce(leg);
					zero += CalculateAdditionalBobForce(leg, standingForceDirectionAndMagnitude, targetHeight);
				}
				Vector3 val3 = ApplyTurningTorqueDamping(leg, inputVector);
				val3 += CalculateTurningTorque(leg, inputVector, deltaTime);
				_rb.AddTorque(CalculateOrientationCorrectionTorque(leg), 1);
				_rb.AddRelativeTorque(val3, 2);
				_rb.AddForceAtPosition(zero * 0.85f, val);
				_rb.AddForceAtPosition(zero * 0.15f + val2, leg.forcePoint);
				_rb.AddTorque(zero2);
			}
		}

		private void ApplyInAirForces(CubeMechLeg leg, Vector4 inputVector, float deltaTime)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = ApplyOrientationCorrectionTorque();
			_rb.AddTorque(val, 1);
			Vector3 val2 = CalculateTurningTorque(leg, inputVector, deltaTime);
			val2 *= leg.movement.inAirTurnAmount;
			val2 += ApplyTurningTorqueDamping(leg, inputVector);
			_rb.AddRelativeTorque(val2, 2);
			if (leg.legData.longJumping && !leg.legData.descending)
			{
				_rb.AddForce(_rbT.get_forward() * leg.legMovement.longJumpForce * _numLegsRatio, 2);
			}
		}

		private Vector3 CalculateOrientationCorrectionTorque(CubeMechLeg leg)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forward = _rbT.get_forward();
			Vector3 right = _rbT.get_right();
			Vector3 alignmentVec = -right;
			float num = Math.Sign(Vector3.Dot(forward, _rb.get_velocity()));
			float num2 = Mathf.Clamp(leg.legData.currentSpeedRatio * num, -0.5f, 1f);
			Vector3 val = Vector3.Lerp(leg.legData.vertHitNormal, leg.legData.footHitNormal, leg.legData.currentSpeedRatio);
			Vector3 val2 = val + Vector3.get_up() + forward * leg.tiltForce * num2;
			Vector3 alignmentVec2 = Vector3.Cross(forward, val2);
			float angleFromAlignment = CalculateAngleFromAlignment(alignmentVec, alignmentVec2, forward);
			Vector3 alignmentVec3 = Vector3.Cross(right, val2);
			float angleFromAlignment2 = CalculateAngleFromAlignment(forward, alignmentVec3, right);
			Vector3 val3 = CalculateAlignmentTorque(forward, angleFromAlignment);
			Vector3 val4 = CalculateAlignmentTorque(right, angleFromAlignment2);
			return val3 + val4;
		}

		private Vector3 ApplyOrientationCorrectionTorque()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			Vector3 forward = _rbT.get_forward();
			Vector3 right = _rbT.get_right();
			Vector3 alignmentVec = -right;
			Vector3 up = Vector3.get_up();
			Vector3 alignmentVec2 = Vector3.Cross(forward, up);
			float angleFromAlignment = CalculateAngleFromAlignment(alignmentVec, alignmentVec2, forward);
			Vector3 alignmentVec3 = Vector3.Cross(right, up);
			float angleFromAlignment2 = CalculateAngleFromAlignment(forward, alignmentVec3, right);
			Vector3 val = CalculateAlignmentTorque(forward, angleFromAlignment);
			Vector3 val2 = CalculateAlignmentTorque(right, angleFromAlignment2);
			return val + val2;
		}

		private Vector3 CalculateAlignmentTorque(Vector3 perpendicularVec, float angleFromAlignment)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = angleFromAlignment * perpendicularVec;
			Vector3 val2 = perpendicularVec * Vector3.Dot(perpendicularVec, _rb.get_angularVelocity());
			val -= val2 * 0.15f;
			val *= _rb.get_mass();
			return val * _numLegsRatio;
		}

		private float CalculateAngleFromAlignment(Vector3 alignmentVec1, Vector3 alignmentVec2, Vector3 perpendicularVec)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Angle(alignmentVec1, alignmentVec2);
			float num2 = Mathf.Sign(Vector3.Dot(perpendicularVec, Vector3.Cross(alignmentVec1, alignmentVec2)));
			return num * (num2 * ((float)Math.PI / 180f));
		}

		private Vector3 CalculateForcePoint(CubeMechLeg leg)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = _rb.get_worldCenterOfMass() - leg.forcePoint;
			float supportRadius = leg.supportRadius;
			Vector3 val2 = (!(val.get_sqrMagnitude() > supportRadius * supportRadius)) ? val : (val.get_normalized() * supportRadius);
			Vector3 val3 = leg.forcePoint + val2;
			Vector3 val4 = val3 - _rb.get_worldCenterOfMass();
			return leg.updatedForcePoint = val3 + leg.up * Vector3.Dot(val4, leg.up);
		}

		private Vector4 GetInputVector(CubeMechLeg leg)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			Vector4 val = _inputVector;
			switch (leg.legData.facingDirection)
			{
			case CubeFace.Front:
				val = Vector4.op_Implicit(Vector3.Reflect(Vector4.op_Implicit(val), Vector3.get_forward()));
				break;
			case CubeFace.Back:
				val = Vector4.op_Implicit(RotateVector(Vector4.op_Implicit(val), 180f, Vector3.get_up()));
				val = Vector4.op_Implicit(Vector3.Reflect(Vector4.op_Implicit(val), Vector3.get_forward()));
				break;
			case CubeFace.Left:
				val = Vector4.op_Implicit(RotateVector(Vector4.op_Implicit(val), -90f, Vector3.get_up()));
				val = Vector4.op_Implicit(Vector3.Reflect(Vector4.op_Implicit(val), Vector3.get_right()));
				break;
			case CubeFace.Right:
				val = Vector4.op_Implicit(RotateVector(Vector4.op_Implicit(val), 90f, Vector3.get_up()));
				val = Vector4.op_Implicit(Vector3.Reflect(Vector4.op_Implicit(val), Vector3.get_right()));
				break;
			}
			return val;
		}

		private Vector3 RotateVector(Vector3 vector, float angle, Vector3 axis)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = Quaternion.AngleAxis(angle, axis);
			return val * vector;
		}

		private void ConvertCubeInputToVector4()
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
			if (_inputVector.y > 0f && !_justJumped)
			{
				_jumpPressedTime += Time.get_deltaTime();
				if (_jumpPressedTime > 0.1f && CheckIfCanJump())
				{
					_fullJumpForcePending = true;
					_halfJumpForcePending = false;
				}
			}
			else if (_inputVector.y <= 0f && !_justJumped)
			{
				if (_jumpPressedTime > 0f)
				{
					if (CheckIfCanJump())
					{
						_halfJumpForcePending = true;
					}
					_jumpPressedTime = 0f;
				}
			}
			else
			{
				_justJumped = false;
			}
		}

		private bool CheckIfCanJump()
		{
			for (int i = 0; i < _legs.Count; i++)
			{
				MechLegData legData = _legs[i].legData;
				if (legData.jumping || legData.legSliding || legData.groundedOnItself || !legData.canJump)
				{
					return false;
				}
			}
			return true;
		}

		private void DecideIfCrouching(CubeMechLeg leg)
		{
			MechLegData legData = leg.legData;
			if (_inputVector.y < 0f && legData.legGrounded)
			{
				legData.targetHeight = leg.movement.idealCrouchingHeight;
				legData.crouching = true;
			}
			else
			{
				legData.targetHeight = leg.movement.idealHeight;
				legData.crouching = false;
			}
		}

		private void ProcessStoppedState(CubeMechLeg leg)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			Vector4 inputVector = _inputVector;
			inputVector.y = 0f;
			if (!legData.stopped)
			{
				if (inputVector.get_sqrMagnitude() < 0.1f && !_isRotating)
				{
					legData.stopped = true;
					legData.stoppedPosition = legData.targetLegPosition;
				}
			}
			else if (inputVector.get_sqrMagnitude() >= 0.1f || _isRotating)
			{
				legData.stopped = false;
			}
		}

		private Vector3 CalculateTooLowForce(CubeMechLeg leg)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			MechLegData legData = leg.legData;
			MechLegMovement movement = leg.movement;
			float num = movement.idealCrouchingHeight - movement.idealHeightRange * 0.5f;
			float num2 = Vector3.Dot(legData.currentVelocity, leg.down);
			if (legData.legGrounded && legData.vertDistToGround < num && num2 > 0f)
			{
				result = leg.up * num2 * _inverseDeltaTime * _rb.get_mass();
				result *= _numLegsRatio;
			}
			return result;
		}

		private Vector3 CalculateStandingForce(CubeMechLeg leg, Vector3 standingForceDirectionAndMagnitude)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return leg.movement.maxUpwardsForce * standingForceDirectionAndMagnitude;
		}

		private Vector3 CalculateLegSpecificStandingForce(CubeMechLeg leg, Vector3 standingForceDirectionAndMagnitude)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			if (standingForceDirectionAndMagnitude == leg.up)
			{
				result = leg.movement.maxUpwardsForce * standingForceDirectionAndMagnitude * 0.5f;
			}
			return result;
		}

		private Vector3 CalculateStandingDamping(CubeMechLeg leg, float targetHeight)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			MechLegMovement movement = leg.movement;
			float num = Vector3.Dot(legData.currentVelocity, leg.up);
			float num2 = 1f - 1f / targetHeight * Mathf.Clamp(Mathf.Abs(legData.vertDistToGround - targetHeight), 0f, targetHeight);
			float num3 = Mathf.Clamp(movement.upwardsDampingForce * num, 0f - movement.maxDampingForce, movement.maxDampingForce);
			return leg.down * num3 * num2;
		}

		private Vector3 CalculateAdditionalBobForce(CubeMechLeg leg, Vector3 standingForceDirectionAndMagnitude, float targetHeight)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			MechLegData legData = leg.legData;
			MechLegMovement movement = leg.movement;
			float num = Vector3.Dot(legData.currentVelocity, leg.right + leg.forward);
			if (Mathf.Abs(targetHeight - legData.vertDistToGround) > 0.2f && Mathf.Abs(num) > 0.1f)
			{
				float num2 = Mathf.Abs(num) / leg.maxSpeed;
				result = movement.maxBobForce * num2 * standingForceDirectionAndMagnitude;
			}
			return result;
		}

		private Vector3 GetStandingForceDirectionAndMagnitude(CubeMechLeg leg, float targetHeight)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			MechLegMovement movement = leg.movement;
			if (legData.vertDistToGround < targetHeight - movement.idealHeightRange)
			{
				return leg.up;
			}
			if (legData.vertDistToGround < targetHeight)
			{
				return leg.up * (targetHeight - legData.vertDistToGround) / movement.idealHeightRange;
			}
			if (legData.vertDistToGround > targetHeight + movement.idealHeightRange)
			{
				return leg.down;
			}
			if (legData.vertDistToGround > targetHeight)
			{
				return leg.down * (legData.vertDistToGround - targetHeight) / movement.idealHeightRange;
			}
			return Vector3.get_zero();
		}

		private Vector3 CalculateTurningTorque(CubeMechLeg leg, Vector4 inputVector, float deltaTime)
		{
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			MechLegMovement movement = leg.movement;
			float num;
			float num2;
			if (_legacyControls)
			{
				num = movement.maxLegacyTurnRate;
				num2 = movement.legacyTurnAcceleration;
			}
			else
			{
				num = movement.maxTurnRate;
				num2 = movement.turnAcceleration;
			}
			float num3 = Vector3.Dot(_rb.get_angularVelocity(), Vector3.get_up());
			Vector3 val;
			if (Mathf.Abs(num3) > num)
			{
				Vector3 angularVelocity = _rb.get_angularVelocity();
				val = angularVelocity.get_normalized() * (num2 * -1f);
			}
			else
			{
				val = CalculateRotationAcceleration(inputVector, num2);
			}
			Vector3 val2 = val * deltaTime;
			return val2 * _numLegsRatio;
		}

		private Vector3 CalculateRotationAcceleration(Vector4 inputVector, float turnAcceleration)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			float num = 0f;
			if (!_legacyControls)
			{
				num = _strafeDirectionManager.angleToStraight;
			}
			if (_legacyControls || _strafeDirectionManager.IsAngleToStraightGreaterThanThreshold())
			{
				if ((_legacyControls && inputVector.x > 0.1f) || num < 0f)
				{
					result = _rbT.get_up() * turnAcceleration;
				}
				else if ((_legacyControls && inputVector.x < -0.1f) || num > 0f)
				{
					result = -(_rbT.get_up() * turnAcceleration);
				}
			}
			return result;
		}

		private Vector3 ApplyTurningTorqueDamping(CubeMechLeg leg, Vector4 inputVector)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			Vector3 angularVelocity = _rb.get_angularVelocity();
			float num = Vector3.Dot(_rbT.get_up(), angularVelocity);
			Vector3 result = Vector3.get_zero();
			if (Mathf.Abs(num) > 0.01f && _legacyControls)
			{
				result = CalculateLegacyTurnDampingTorque(leg.movement, inputVector, num);
			}
			return result;
		}

		private Vector3 CalculateLegacyTurnDampingTorque(MechLegMovement movement, Vector4 inputVector, float rotationSpeed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			if (Mathf.Abs(inputVector.x) < 0.1f)
			{
				result = _rbT.get_up() * (0f - rotationSpeed) * movement.newTurnDampingScale * movement.legacyTurnDampingScale;
			}
			return result;
		}

		private Vector3 CalculateLateralForce(CubeMechLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			Vector3 result = Vector3.get_zero();
			if (_legacyControls)
			{
				result = CalculateLegacyLateralForce(leg, inputVector);
			}
			else if (!_legacyControls)
			{
				result = CalculateNewLateralForce(leg, inputVector);
			}
			return result;
		}

		private Vector3 CalculateLegacyLateralForce(CubeMechLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			if (inputVector.z > 0.1f)
			{
				val += ApplyLateralForceVector(leg, leg.back, leg.movement.maxLateralForce, Mathf.Abs(inputVector.z));
			}
			else if (inputVector.z < -0.1f)
			{
				val += ApplyLateralForceVector(leg, leg.forward, leg.movement.maxLateralForce, Mathf.Abs(inputVector.z));
			}
			if (inputVector.w > 0.1f)
			{
				val += ApplyLateralForceVector(leg, leg.left, leg.movement.maxLateralForce);
			}
			else if (inputVector.w < -0.1f)
			{
				val += ApplyLateralForceVector(leg, leg.right, leg.movement.maxLateralForce);
			}
			return val;
		}

		private Vector3 CalculateNewLateralForce(CubeMechLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			Vector3 forwardMovementDirection = _strafeDirectionManager.forwardMovementDirection;
			Vector3 rightMovementDirection = _strafeDirectionManager.rightMovementDirection;
			if (inputVector.z > 0.1f)
			{
				val += ApplyLateralForceVector(leg, forwardMovementDirection, leg.movement.maxLateralForce, Mathf.Abs(inputVector.z));
			}
			else if (inputVector.z < -0.1f)
			{
				val += ApplyLateralForceVector(leg, -forwardMovementDirection, leg.movement.maxLateralForce, Mathf.Abs(inputVector.z));
			}
			if (inputVector.x > 0.1f)
			{
				val += ApplyLateralForceVector(leg, rightMovementDirection, leg.movement.maxLateralForce, Mathf.Abs(inputVector.x));
			}
			else if (inputVector.x < -0.1f)
			{
				val += ApplyLateralForceVector(leg, -rightMovementDirection, leg.movement.maxLateralForce, Mathf.Abs(inputVector.x));
			}
			return val;
		}

		private Vector3 CalculateLateralDampingForce(CubeMechLeg leg, Vector4 inputVector)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			if (inputVector.z < 0.1f && inputVector.z > -0.1f)
			{
				val += ApplyLateralDampingVector(leg, leg.forward, leg.movement.lateralDampForce);
			}
			if (inputVector.x < 0.1f && inputVector.x > -0.1f && inputVector.w < 0.1f && inputVector.w > -0.1f && !_isRotating)
			{
				val += ApplyLateralDampingVector(leg, leg.left, leg.movement.lateralDampForce);
			}
			return val;
		}

		private Vector3 ApplyLateralForceVector(CubeMechLeg leg, Vector3 vector, float maxForce, float sidewaysModifer = 1f)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			float num = maxForce * sidewaysModifer;
			return vector * num;
		}

		private Vector3 ApplyLateralDampingVector(CubeMechLeg leg, Vector3 vector, float damping)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			MechLegMovement movement = leg.movement;
			float num = Vector3.Dot(leg.legData.currentVelocity, vector);
			float num2 = Mathf.Clamp(damping * num, 0f - movement.maxDampingForce, movement.maxDampingForce);
			return -vector * num2;
		}

		private Vector3 CalculateStoppedForce(CubeMechLeg leg)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			Vector3 result = Vector3.get_zero();
			if (legData.stopped)
			{
				float num = 1f / leg.validLegRadius;
				float num2 = Vector3.Dot(leg.right, legData.stoppedPosition - leg.position);
				float num3 = num2 * num;
				float num4 = Vector3.Dot(leg.forward, legData.stoppedPosition - leg.position);
				float num5 = num4 * num;
				result = (leg.right * num3 + leg.forward * num5) * leg.movement.maxStoppedForce;
			}
			return result;
		}

		private void ProcessJumpForce(float deltaTime)
		{
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_016d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0214: Unknown result type (might be due to invalid IL or missing references)
			bool flag = false;
			bool flag2 = true;
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeMechLeg cubeMechLeg = _legs[i];
				flag |= cubeMechLeg.legData.jumping;
				flag2 &= cubeMechLeg.legData.canJump;
			}
			if (!flag)
			{
				if ((!_fullJumpForcePending && !_halfJumpForcePending) || !flag2)
				{
					return;
				}
				_justJumped = true;
				float num = 1f;
				float num2 = 0f;
				float num3 = 0f;
				for (int j = 0; j < _legs.Count; j++)
				{
					CubeMechLeg cubeMechLeg2 = _legs[j];
					num2 += cubeMechLeg2.movement.jumpHeight;
					num *= cubeMechLeg2.movement.longJumpForce;
					num3 += cubeMechLeg2.legData.currentSpeedRatio;
				}
				float num4 = num2 / (float)_legs.Count;
				num3 /= (float)_legs.Count;
				bool flag3 = num > 0f && num3 > 0.8f && !_halfJumpForcePending;
				if (flag3)
				{
					Vector3 velocity = _rb.get_velocity();
					velocity.y = 0f;
					velocity.Normalize();
					Vector3 val = Vector3.Cross(_rbT.get_right(), Vector3.get_up());
					if (Vector3.Dot(velocity, val) < 0.5f)
					{
						flag3 = false;
					}
				}
				RobotJumped(flag3);
				if (_halfJumpForcePending)
				{
					num4 *= 0.5f;
				}
				float num5 = 2f * num4;
				Vector3 gravity = Physics.get_gravity();
				float num6 = Mathf.Sqrt(num5 * gravity.get_magnitude());
				if (Vector3.Angle(Vector3.get_down(), -_rbT.get_up()) < 70f)
				{
					_rb.AddForce(Vector3.get_up() * num6, 2);
				}
				else
				{
					_rb.AddForce(_rbT.get_up() * num6, 2);
				}
				_fullJumpForcePending = false;
				_halfJumpForcePending = false;
			}
			else
			{
				if (_justJumped)
				{
					return;
				}
				_jumpTimeRemaining -= deltaTime;
				if (!(_jumpTimeRemaining <= 0f))
				{
					return;
				}
				for (int k = 0; k < _legs.Count; k++)
				{
					MechLegData legData = _legs[k].legData;
					if (legData.legGrounded)
					{
						legData.jumping = false;
					}
				}
			}
		}

		private void RobotJumped(bool longJump)
		{
			_jumpTimeRemaining = _jumpDuration;
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeMechLeg cubeMechLeg = _legs[i];
				cubeMechLeg.legData.jumping = true;
				cubeMechLeg.legGraphics.justJumped = true;
				cubeMechLeg.legData.longJumping = (longJump && cubeMechLeg.legMovement.longJumpForce > 0f);
			}
		}

		private void ProcessInAirRotation(float deltaTime)
		{
			bool flag = false;
			for (int i = 0; i < _legs.Count; i++)
			{
				MechLegData legData = _legs[i].legData;
				if (legData.legGrounded)
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
				Quaternion localRotation = _rbT.get_localRotation();
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
				Vector3 val = Quaternion.Inverse(_rbT.get_rotation()) * (_rb.get_angularVelocity() * 57.29578f);
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
						if (Mathf.Abs(val3.get_Item(j)) > val2.get_Item(j))
						{
							zero.set_Item(j, (0f - num2) * deltaTime * Mathf.Sign(val.get_Item(j)));
						}
						else
						{
							zero.set_Item(j, (0f - num2) * deltaTime * Mathf.Sign(Mathf.Abs(val.get_Item(j)) - num2));
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
	}
}
