using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal abstract class LegManager
	{
		protected LegGraphicsManager _legGraphics;

		protected List<CubeLeg> _legs = new List<CubeLeg>();

		protected float _distToGround = float.MaxValue;

		protected bool _justJumped;

		protected int _raycastMask = GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK;

		private const float VALID_TERRAIN_ANGLE = 0.087156f;

		public virtual void RegisterLeg(CubeLeg leg)
		{
			leg.legData.initialised = false;
			_legs.Add(leg);
			_legGraphics.RegisterLeg(leg);
		}

		public virtual void UnregisterLeg(CubeLeg leg)
		{
			_legs.Remove(leg);
			_legGraphics.UnregisterLeg(leg);
		}

		public abstract void Tick(float deltaTime);

		public abstract void PhysicsTick(float deltaTime);

		public virtual void SetJumpState()
		{
		}

		internal virtual void Unregister()
		{
			_legGraphics.Unregister();
		}

		protected void ProcessCurrentVelocity(CubeLeg leg, float deltaTime)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			leg.legData.currentVelocity = (leg.position - leg.legData.lastPosition) / deltaTime;
			leg.legData.currentSpeed = leg.legData.currentVelocity.get_magnitude();
			leg.legData.lastPosition = leg.position;
		}

		protected void UpdateTargetGroundedPosition(CubeLeg leg, float deltaTime)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0121: Unknown result type (might be due to invalid IL or missing references)
			//IL_0127: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0149: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			if (!_justJumped)
			{
				float num = Mathf.Clamp01(leg.legData.currentSpeed / leg.maxSpeed);
				Vector3 val = leg.position + leg.forward * leg.footOffsetToLegBase;
				Vector3 val2 = leg.down;
				if (!leg.legData.stopped)
				{
					val2 += leg.legData.currentVelocity.get_normalized() * leg.validLegRadius * num;
				}
				if (RayCast(leg, val, val2, leg.maxLegLength, out RaycastHit hit, deltaTime))
				{
					leg.legData.legGrounded = true;
					_distToGround = Vector3.Distance(val, hit.get_point());
					return;
				}
				val = leg.position + leg.forward * leg.footOffsetToLegBase * 0.25f;
				float num2 = Vector3.Dot(leg.forward, leg.legData.currentVelocity);
				Vector3 val3 = leg.legData.currentVelocity - leg.forward * num2;
				Quaternion val4 = Quaternion.AngleAxis(45f * num, Vector3.Cross(val3.get_normalized(), leg.down));
				Vector3 direction = val4 * leg.down;
				if (RayCast(leg, val, direction, leg.maxLegLength, out hit, deltaTime))
				{
					leg.legData.legGrounded = true;
					_distToGround = Vector3.Distance(leg.position, hit.get_point());
					return;
				}
			}
			leg.legData.legGrounded = false;
			_distToGround = float.MaxValue;
		}

		internal virtual void DisableLegs()
		{
		}

		internal virtual void EnableLegs()
		{
		}

		private bool RayCast(CubeLeg leg, Vector3 position, Vector3 direction, float raycastDistance, out RaycastHit hit, float deltaTime)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			if (Physics.Raycast(position, direction, ref hit, raycastDistance, _raycastMask) && IsHitNormalValid(hit, direction))
			{
				if ((leg.legGraphics.isAnimating && !leg.legGraphics.quietAnimation) || !IsCurrentLegPositionValid(leg, hit.get_point(), leg.validLegRadius, deltaTime))
				{
					leg.legData.targetLegPosition = hit.get_point() + leg.legData.currentVelocity * deltaTime;
					if (leg.legData.stopped)
					{
						leg.legData.stoppedPosition = leg.legData.targetLegPosition;
					}
				}
				return true;
			}
			return false;
		}

		private bool IsCurrentLegPositionValid(CubeLeg leg, Vector3 hitPoint, float validRadius, float deltaTime)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = leg.position + leg.forward * leg.footOffsetToLegBase + leg.down * leg.legData.targetHeight - leg.legData.targetLegPosition + leg.legData.currentVelocity * deltaTime;
			return val.get_sqrMagnitude() <= validRadius * validRadius;
		}

		private bool IsHitNormalValid(RaycastHit hit, Vector3 direction)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			float num = Vector3.Dot(hit.get_normal(), direction);
			return num < 0.087156f;
		}
	}
}
