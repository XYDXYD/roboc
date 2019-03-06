using Simulation.Hardware.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal abstract class MechLegManager
	{
		protected MechLegGraphicsManager _legGraphicsManager;

		protected List<CubeMechLeg> _legs = new List<CubeMechLeg>();

		protected NetworkMachineManager _machineManager;

		protected Rigidbody _rb;

		protected float _numLegsRatio;

		protected float _mass;

		protected bool _justJumped;

		protected float _jumpTimeRemaining;

		protected float _jumpDuration = 0.6f;

		protected int _raycastMask = GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK;

		private GridAllignedLineCheck.GridAlignedCheckDependency _gridAlignedCheckDependency = new GridAllignedLineCheck.GridAlignedCheckDependency();

		private HitResult[] _hitResults = new HitResult[1];

		private int _machineId;

		public const float STOPPED_ROTATION_SPEED = 0.01f;

		private const float VALID_TERRAIN_ANGLE = 0.08660254f;

		private const float VALID_TWISTED_LEG_ANGLE = 0.9396f;

		private const float MAX_VALID_WALKING_SLOPE = 60f;

		private const float MAX_VALID_SLIDING_SLOPE = 80f;

		private const float MAX_STEP_ANGLE = 70f;

		public const float LONG_JUMP_SPEED_TRESHOLD = 0.8f;

		public MechLegManager(Rigidbody rigidBody, NetworkMachineManager machineManger, bool localPlayer)
		{
			_rb = rigidBody;
			_machineManager = machineManger;
			_legGraphicsManager = new MechLegGraphicsManager(_rb.get_gameObject(), localPlayer);
		}

		public virtual void RegisterLeg(CubeMechLeg leg)
		{
			leg.legData.initialised = false;
			_legs.Add(leg);
			_legGraphicsManager.RegisterLeg(leg);
		}

		public virtual void UnregisterLeg(CubeMechLeg leg)
		{
			_legs.Remove(leg);
			_legGraphicsManager.UnregisterLeg(leg);
		}

		public void SetMachineId(int machineId)
		{
			_machineId = machineId;
		}

		public abstract void Tick(float deltaTime);

		public abstract void PhysicsTick(float deltaTime);

		public virtual void SetJumpState()
		{
		}

		internal virtual void PauseLegs(bool value)
		{
		}

		internal virtual void DisableMechLegs()
		{
		}

		internal virtual void EnableMechLegs()
		{
		}

		internal virtual void Unregister()
		{
			_legGraphicsManager.Unregister();
		}

		protected void UpdateGraphics(float deltaTime)
		{
			_legGraphicsManager.Tick(deltaTime);
		}

		protected void ProcessCurrentVelocity(CubeMechLeg leg, float deltaTime)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			legData.previousVelocity = legData.currentVelocity;
			legData.currentVelocity = (leg.position - legData.lastPosition) / deltaTime;
			legData.currentSpeed = legData.currentVelocity.get_magnitude();
			legData.currentSpeedRatio = Mathf.Clamp01(legData.currentSpeed / leg.maxSpeed);
			legData.lastPosition = leg.position;
		}

		protected void CheckIsDescending(CubeMechLeg leg)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			if (leg.legData.jumping && !leg.legData.descending && !leg.legData.legGrounded && _jumpDuration - _jumpTimeRemaining > 0.1f)
			{
				float y = leg.legData.lastPosition.y;
				Vector3 position = leg.position;
				if (y > position.y)
				{
					leg.legGraphics.justDescending = true;
					leg.legData.descending = true;
				}
			}
		}

		protected void UpdateGroundedOnItself(CubeMechLeg leg)
		{
			float num = leg.colliders[0].get_radius() * 2f;
			Vector3 hitPoint = leg.position + leg.down * num;
			Byte3 value = (Byte3)GridScaleUtility.WorldToGrid(leg.get_transform().get_localPosition(), TargetType.Player);
			_gridAlignedCheckDependency.Populate(hitPoint, _rb, leg.position, leg.down, leg.maxLegLength, _machineManager.GetMachineMap(TargetType.Player, _machineId), TargetType.Player, value);
			int cubeInGridStepLine = GridAllignedLineCheck.GetCubeInGridStepLine(_gridAlignedCheckDependency, _hitResults);
			if (cubeInGridStepLine > 0)
			{
				MechLegData legData = leg.legData;
				legData.groundedOnItself = true;
				legData.targetLegPosition = GridScaleUtility.GetCubeWorldPosition(_hitResults[0].gridHit.hitGridPos, _rb, TargetType.Player);
				ref Vector3 targetLegPosition = ref legData.targetLegPosition;
				Vector3 position = leg.position;
				float x = position.x;
				float y = legData.targetLegPosition.y;
				Vector3 position2 = leg.position;
				targetLegPosition.Set(x, y, position2.z);
				leg.footPosition = legData.targetLegPosition;
			}
			else
			{
				leg.legData.groundedOnItself = false;
			}
		}

		protected void UpdateVerticalGroundPosition(CubeMechLeg leg, float deltaTime)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			if (_justJumped)
			{
				return;
			}
			MechLegData legData = leg.legData;
			Vector3 val = Vector3.Lerp(leg.down, -leg.legData.footHitNormal, leg.legData.currentSpeedRatio);
			bool flag = true;
			if (legData.stopped && !legData.legSliding)
			{
				Vector3 val2 = legData.stoppedPosition - leg.forcePoint;
				Vector3 normalized = val2.get_normalized();
				if (Vector3.Dot(normalized, val) > 0.9396f)
				{
					flag = false;
					val = legData.stoppedPosition - leg.forcePoint;
				}
			}
			if (!RayCast(leg.position, val, leg.maxLegLength * 2f, out RaycastHit hit) || (!legData.terrainWalkingAngleValid && !legData.terrainSlidingAngleValid))
			{
				return;
			}
			legData.vertHitNormal = hit.get_normal();
			legData.vertDistToGround = Vector3.Distance(leg.position, hit.get_point());
			if (legData.terrainSlidingAngleValid || leg.isHidden || flag)
			{
				if (leg.isHidden)
				{
					leg.legGraphics.targetFootPos = hit.get_point();
				}
				legData.targetLegPosition = hit.get_point();
				legData.stoppedPosition = hit.get_point();
			}
		}

		protected void UpdateTargetGroundedPosition(CubeMechLeg leg, float deltaTime)
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			float maxLegLength = leg.maxLegLength;
			MechLegData legData = leg.legData;
			if (!_justJumped)
			{
				float radius = leg.colliders[0].get_radius();
				Vector3 position = leg.position + leg.down * radius;
				maxLegLength -= radius;
				Vector3 val = leg.down;
				float currentSpeedRatio = legData.currentSpeedRatio;
				if (!legData.stopped && Mathf.Abs(legData.currentSpeed) > 0.1f)
				{
					val += legData.currentVelocity.get_normalized() * (leg.validLegRadius * 0.5f * leg.percentageOfForwardStride) * currentSpeedRatio;
				}
				if (RaycastForTargetPosition(leg, position, val, maxLegLength, deltaTime))
				{
					return;
				}
				if (Vector3.Angle(Vector3.get_down(), leg.down) < 70f)
				{
					float num = Vector3.Dot(leg.forward, legData.currentVelocity);
					Vector3 val2 = legData.currentVelocity + leg.forward * num;
					Quaternion val3 = Quaternion.AngleAxis(45f * currentSpeedRatio, Vector3.Cross(val2.get_normalized(), leg.down));
					Vector3 direction = val3 * leg.down;
					if (RaycastForTargetPosition(leg, position, direction, maxLegLength, deltaTime))
					{
						return;
					}
				}
			}
			legData.legGrounded = false;
			legData.legLanding = false;
		}

		private bool RaycastForTargetPosition(CubeMechLeg leg, Vector3 position, Vector3 direction, float raycastDistance, float deltaTime)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			if (RayCast(position, direction, raycastDistance, out RaycastHit hit))
			{
				CheckCurrentTerrainAngle(leg, hit);
				if (legData.terrainWalkingAngleValid || legData.legGrounded)
				{
					CheckLegLanding(leg);
					CheckLegSliding(leg);
					SetLegTargetPosition(leg, hit, deltaTime);
					return true;
				}
			}
			return false;
		}

		private bool RayCast(Vector3 position, Vector3 direction, float raycastDistance, out RaycastHit hit)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (Physics.Raycast(position, direction, ref hit, raycastDistance, _raycastMask) && IsHitNormalValid(hit, direction))
			{
				return true;
			}
			return false;
		}

		private bool IsCurrentLegPositionValid(CubeMechLeg leg, Vector3 hitPoint, float validRadius, float deltaTime)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			Vector3 val = leg.footPosition - legData.targetLegPosition + legData.currentVelocity * deltaTime;
			if (legData.stopped && val.get_sqrMagnitude() <= validRadius * validRadius)
			{
				return true;
			}
			return false;
		}

		private bool IsHitNormalValid(RaycastHit hit, Vector3 direction)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Dot(hit.get_normal(), direction);
			if (!(num < 0.08660254f))
			{
				return false;
			}
			Vector3.Angle(Vector3.get_forward(), hit.get_normal());
			return num < 0.08660254f;
		}

		private void CheckLegSliding(CubeMechLeg leg)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			if (!legData.legShouldSlide && legData.terrainSlidingAngleValid)
			{
				legData.legShouldSlide = true;
				legData.slideStartVelocity = -legData.currentVelocity;
			}
			else
			{
				if (!legData.legShouldSlide || legData.legSliding)
				{
					return;
				}
				if (legData.terrainSlidingAngleValid)
				{
					float num = Vector3.Dot(legData.slideStartVelocity, legData.currentVelocity);
					if (num >= 0f)
					{
						legData.legSliding = true;
					}
				}
				else if (legData.terrainWalkingAngleValid)
				{
					legData.legShouldSlide = false;
				}
			}
		}

		private void CheckCurrentTerrainAngle(CubeMechLeg leg, RaycastHit hit)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			float num = Vector3.Dot(Vector3.get_up(), hit.get_normal());
			num = Mathf.Clamp(num, -1f, 1f);
			float num2 = Mathf.Acos(num);
			num2 *= 57.29578f;
			float num3 = 60f;
			if (legData.legSliding)
			{
				num3 -= 5f;
			}
			if (num2 < num3)
			{
				legData.terrainWalkingAngleValid = true;
				legData.terrainSlidingAngleValid = false;
				legData.legSliding = false;
			}
			else if (num2 < 80f)
			{
				legData.terrainWalkingAngleValid = false;
				legData.terrainSlidingAngleValid = true;
			}
			else
			{
				legData.terrainWalkingAngleValid = false;
				legData.terrainSlidingAngleValid = false;
				legData.legGrounded = false;
				legData.legSliding = false;
			}
		}

		private void CheckLegLanding(CubeMechLeg leg)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			if (!legData.legGrounded)
			{
				leg.legGraphics.currentTransitionTime = 0f;
				legData.legLanding = true;
			}
			float num = Vector3.Dot(legData.currentVelocity, Vector3.get_up());
			if (!legData.legGrounded && num < -2f)
			{
				leg.legGraphics.justLanded = true;
			}
		}

		private void SetLegTargetPosition(CubeMechLeg leg, RaycastHit hitInfo, float deltaTime)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			legData.legGrounded = true;
			legData.footHitNormal = hitInfo.get_normal();
			if ((leg.legGraphics.isAnimating && !leg.legGraphics.quietAnimation) || !IsCurrentLegPositionValid(leg, hitInfo.get_point(), leg.validLegRadius * 0.5f, deltaTime))
			{
				legData.targetLegPosition = hitInfo.get_point();
				if (legData.stopped)
				{
					legData.stoppedPosition = legData.targetLegPosition;
				}
			}
		}

		protected void ProcessLegMass()
		{
			if (Mathf.RoundToInt(_rb.get_mass()) != Mathf.RoundToInt(_mass))
			{
				UpdateLegMass();
			}
		}

		protected void UpdateLegMass()
		{
			if (_rb != null)
			{
				_numLegsRatio = 1f / (float)_legs.Count;
				_mass = _rb.get_mass();
				float massPerLeg = _mass / (float)_legs.Count;
				for (int i = 0; i < _legs.Count; i++)
				{
					CubeMechLeg cubeMechLeg = _legs[i];
					cubeMechLeg.legData.massPerLeg = massPerLeg;
				}
			}
		}
	}
}
