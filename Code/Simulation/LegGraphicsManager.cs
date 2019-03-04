using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class LegGraphicsManager
	{
		private LegAudioManager _legAudioManager;

		private const float MIN_MOVE_THRESHOLD = 0.1f;

		private const float SAME_PLACE_THRESHOLD = 0.05f;

		private float _maxAnimationPeriod = 0.4f;

		private float _animationPeriod = 0.4f;

		private float _timeTillAnimationUpdate;

		private int _currentAnimationGroup;

		private List<CubeLeg> _legs = new List<CubeLeg>();

		public LegGraphicsManager(GameObject robotObj)
		{
			_legAudioManager = new LegAudioManager(robotObj);
		}

		public void RegisterLeg(CubeLeg cube)
		{
			_legs.Add(cube);
		}

		public void UnregisterLeg(CubeLeg cube)
		{
			_legs.Remove(cube);
			if (_legs.Count == 0)
			{
				_legAudioManager.UpdateLoopingValues(_legs);
			}
		}

		public void Tick(float deltaTime)
		{
			UpdateAnimationGroup(deltaTime);
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeLeg cubeLeg = _legs[i];
				if (cubeLeg.T == null)
				{
					return;
				}
				UpdateLeg(cubeLeg, deltaTime);
			}
			_legAudioManager.UpdateLoopingValues(_legs);
		}

		private void UpdateAnimationGroup(float deltaTime)
		{
			CalculateAnimationPeriod();
			_timeTillAnimationUpdate += deltaTime;
			if (!(_timeTillAnimationUpdate >= _animationPeriod))
			{
				return;
			}
			_timeTillAnimationUpdate = 0f;
			if (++_currentAnimationGroup > 1)
			{
				_currentAnimationGroup = 0;
			}
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeLeg cubeLeg = _legs[i];
				if (cubeLeg.legGraphics.syncGroup == _currentAnimationGroup)
				{
					cubeLeg.legGraphics.canAnimate = true;
				}
				else if (!cubeLeg.legGraphics.isAnimating)
				{
					cubeLeg.legGraphics.canAnimate = false;
				}
			}
		}

		internal void Unregister()
		{
			_legAudioManager.StopSound();
		}

		private void CalculateAnimationPeriod()
		{
			float num = 0f;
			float num2 = float.MaxValue;
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeLeg cubeLeg = _legs[i];
				num = Mathf.Max(num, Mathf.Max(cubeLeg.legData.currentSpeed, cubeLeg.maxSpeed));
				num2 = Mathf.Min(num2, cubeLeg.validLegRadius * 2f);
			}
			_animationPeriod = Mathf.Min(num2 / num, _maxAnimationPeriod);
		}

		private void UpdateLeg(CubeLeg leg, float deltaTime)
		{
			UpdateVisibleState(leg);
			UpdateAnimationState(leg);
			UpdateAnimation(leg, deltaTime);
			UpdateNonGroundedState(leg, deltaTime);
		}

		private void UpdateVisibleState(CubeLeg leg)
		{
			if (leg.wasHidden != leg.isHidden)
			{
				leg.wasHidden = leg.isHidden;
				leg.ik.set_enabled(!leg.isHidden);
			}
		}

		private void UpdateAnimationState(CubeLeg leg)
		{
			if (!leg.isHidden && leg.legGraphics.canAnimate && !leg.legGraphics.legRetracted)
			{
				if (ShouldAnimateFully(leg))
				{
					Animate(leg);
				}
				else if (ShouldAnimateQuietly(leg))
				{
					leg.legGraphics.quietAnimation = true;
					Animate(leg);
				}
			}
		}

		private bool ShouldAnimateFully(CubeLeg leg)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = leg.legData.targetLegPosition - leg.footPosition;
			float sqrMagnitude = val.get_sqrMagnitude();
			return sqrMagnitude > 0.0100000007f;
		}

		private bool ShouldAnimateQuietly(CubeLeg leg)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = leg.legData.targetLegPosition - leg.exactFootPosition;
			float sqrMagnitude = val.get_sqrMagnitude();
			return sqrMagnitude > 0.00250000018f;
		}

		private void Animate(CubeLeg leg)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			if (!leg.legGraphics.isAnimating)
			{
				leg.legGraphics.totalTransitionTime = Mathf.Max(_animationPeriod - _timeTillAnimationUpdate, leg.animationTimeFastStep);
				leg.legGraphics.currentTransitionTime = 0f;
				Vector3 startPos = Quaternion.Inverse(leg.T.get_rotation()) * (leg.footPosition - leg.position);
				leg.legGraphics.startPos = startPos;
				if (!leg.legGraphics.quietAnimation)
				{
					_legAudioManager.StartStep(leg);
				}
			}
			leg.legGraphics.targetPos = leg.legData.targetLegPosition;
			leg.legGraphics.isAnimating = true;
		}

		private void UpdateAnimation(CubeLeg leg, float deltaTime)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			if (!leg.legGraphics.isAnimating)
			{
				return;
			}
			leg.legGraphics.currentTransitionTime += deltaTime;
			if (leg.legGraphics.currentTransitionTime < leg.legGraphics.totalTransitionTime)
			{
				float num = leg.footMovementCurve.Evaluate(leg.legGraphics.animationProgress);
				Vector3 val = leg.T.get_rotation() * leg.legGraphics.startPos + leg.position;
				Vector3 val2 = val;
				Vector3 val3 = (leg.legGraphics.targetPos - val) * num;
				Vector3 val4 = Vector3.get_zero();
				if (!leg.legGraphics.quietAnimation)
				{
					val4 = leg.up * leg.animationHeight * leg.footHeightCurve.Evaluate(num);
				}
				leg.footPosition = val2 + val3 + val4;
			}
			else
			{
				if (leg.legData.legGrounded && !leg.legGraphics.quietAnimation)
				{
					_legAudioManager.EndStep(leg);
				}
				leg.footPosition = leg.legGraphics.targetPos;
				leg.legGraphics.canAnimate = false;
				leg.legGraphics.isAnimating = false;
				leg.legGraphics.quietAnimation = false;
			}
		}

		private void UpdateNonGroundedState(CubeLeg leg, float deltaTime)
		{
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			if (!leg.isHidden && !leg.legData.legGrounded)
			{
				if (!leg.legGraphics.legRetracted)
				{
					leg.legGraphics.currentTransitionTime = 0f;
					leg.legGraphics.totalTransitionTime = leg.animationTimeSlowStep;
					Vector3 startPos = Quaternion.Inverse(leg.T.get_rotation()) * (leg.footPosition - leg.position);
					leg.legGraphics.startPos = startPos;
					leg.legGraphics.targetPos = leg.retractedPosition;
					leg.legGraphics.isAnimating = true;
					leg.legGraphics.legRetracted = true;
				}
				else if (leg.legGraphics.isAnimating)
				{
					leg.legGraphics.targetPos = leg.retractedPosition;
				}
				else
				{
					leg.footPosition = leg.retractedPosition;
				}
			}
			else
			{
				leg.legGraphics.legRetracted = false;
			}
		}
	}
}
