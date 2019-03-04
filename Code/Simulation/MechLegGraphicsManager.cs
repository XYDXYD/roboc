using Svelto.DataStructures;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class MechLegGraphicsManager
	{
		private MechLegAudioManager _legAudioManager;

		private MechLegEffectsManager _legEffectsManager;

		private const float MIN_MOVE_THRESHOLD = 0.6f;

		private const float SAME_PLACE_THRESHOLD = 0.15f;

		private float _maxAnimationPeriod = 0.5f;

		private float _animationPeriod = 0.3f;

		private float _timeTillAnimationUpdate;

		private int _currentAnimationGroup;

		private int _numSyncGroups;

		private FasterList<CubeMechLeg> _legs = new FasterList<CubeMechLeg>();

		public MechLegGraphicsManager(GameObject robotObj, bool isLocalPlayer)
		{
			_legAudioManager = new MechLegAudioManager(robotObj, isLocalPlayer);
			_legEffectsManager = new MechLegEffectsManager();
		}

		public void RegisterLeg(CubeMechLeg leg)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			_legs.Add(leg);
			leg.footPosition = leg.jumpingRetractedPosition;
			leg.legData.jumping = true;
			leg.legData.legGrounded = false;
			RecalculateLegSyncGroups();
		}

		public void UnregisterLeg(CubeMechLeg leg)
		{
			StopAllEffects(leg);
			_legs.Remove(leg);
			if (_legs.get_Count() == 0)
			{
				_legAudioManager.StopAllLoops();
			}
			RecalculateLegSyncGroups();
		}

		private void StopAllEffects(CubeMechLeg leg)
		{
			_legEffectsManager.HideSkiddingEffect(leg);
		}

		private void UpdateNumLegSyncGroups()
		{
			_numSyncGroups = 1;
		}

		private void RecalculateLegSyncGroups()
		{
			int syncGroup = 0;
			bool flag = true;
			bool flag2 = false;
			UpdateNumLegSyncGroups();
			int num;
			for (num = 0; num < _legs.get_Count(); num++)
			{
				List<CubeMechLeg> list = CreateAndSortLegRowList(num);
				syncGroup = SetSyncGroups(list, syncGroup, flag);
				if (flag2 || list.Count > 1)
				{
					flag = !flag;
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
				num += list.Count - 1;
			}
		}

		internal void Unregister()
		{
			_legAudioManager.StopAllLoops();
		}

		private int SetSyncGroups(List<CubeMechLeg> legRowList, int syncGroup, bool iterateForwards)
		{
			if (iterateForwards)
			{
				for (int i = 0; i < legRowList.Count; i++)
				{
					legRowList[i].legGraphics.syncGroup = syncGroup;
					if (++syncGroup > _numSyncGroups)
					{
						syncGroup = 0;
					}
				}
			}
			else
			{
				for (int num = legRowList.Count - 1; num >= 0; num--)
				{
					legRowList[num].legGraphics.syncGroup = syncGroup;
					if (++syncGroup > _numSyncGroups)
					{
						syncGroup = 0;
					}
				}
			}
			return syncGroup;
		}

		private List<CubeMechLeg> CreateAndSortLegRowList(int currentLegIndex)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position = _legs.get_Item(currentLegIndex).position;
			float z = position.z;
			List<CubeMechLeg> list = new List<CubeMechLeg>();
			list.Clear();
			for (int i = currentLegIndex; i < _legs.get_Count(); i++)
			{
				CubeMechLeg cubeMechLeg = _legs.get_Item(i);
				Vector3 position2 = cubeMechLeg.position;
				if (Mathf.Abs(position2.z - z) < 0.001f)
				{
					list.Add(cubeMechLeg);
					continue;
				}
				break;
			}
			list.Sort(delegate(CubeMechLeg a, CubeMechLeg b)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position3 = a.position;
				ref float x = ref position3.x;
				Vector3 position4 = b.position;
				return x.CompareTo(position4.x);
			});
			return list;
		}

		public void Tick(float deltaTime)
		{
			UpdateAnimationGroup(deltaTime);
			_legAudioManager.UpdateLoopingValues(_legs);
			for (int i = 0; i < _legs.get_Count(); i++)
			{
				CubeMechLeg cubeMechLeg = _legs.get_Item(i);
				if (cubeMechLeg.T == null)
				{
					break;
				}
				UpdateLeg(cubeMechLeg, deltaTime);
			}
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
			if (++_currentAnimationGroup > _numSyncGroups)
			{
				_currentAnimationGroup = 0;
			}
			for (int i = 0; i < _legs.get_Count(); i++)
			{
				MechLegGraphics legGraphics = _legs.get_Item(i).legGraphics;
				if (legGraphics.syncGroup == _currentAnimationGroup)
				{
					legGraphics.canAnimate = true;
				}
				else if (!legGraphics.isAnimating)
				{
					legGraphics.canAnimate = false;
				}
			}
		}

		private void CalculateAnimationPeriod()
		{
			float num = 0f;
			float num2 = float.MaxValue;
			float num3 = 0f;
			for (int i = 0; i < _legs.get_Count(); i++)
			{
				CubeMechLeg cubeMechLeg = _legs.get_Item(i);
				num = Mathf.Max(Mathf.Max(num, cubeMechLeg.legData.currentSpeed), cubeMechLeg.maxSpeed);
				float num4 = cubeMechLeg.validLegRadius * cubeMechLeg.percentageOfStrideLength;
				num2 = Mathf.Min(num2, num4);
				if (cubeMechLeg.runningAnimationHeight > 0f)
				{
					num3 = Mathf.Max(num4, num3);
				}
			}
			num3 /= num;
			_animationPeriod = Mathf.Clamp(num2 / num, num3, _maxAnimationPeriod);
			if (_numSyncGroups == 2)
			{
				_animationPeriod *= 0.6f;
			}
		}

		private void UpdateLeg(CubeMechLeg leg, float deltaTime)
		{
			if (!leg.legData.groundedOnItself)
			{
				UpdateVisibleState(leg);
				UpdateNonGroundedState(leg, deltaTime);
				UpdateAnimationState(leg);
				UpdateAnimation(leg, deltaTime);
				PlaceFoot(leg);
			}
			_legEffectsManager.EnableRunningEffect(leg, leg.legMovement.longJumpForce > 0f && leg.legData.legGrounded && leg.legData.currentSpeedRatio > 0.8f);
		}

		private void PlaceFoot(CubeMechLeg leg)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			if (!legData.jumping)
			{
				Vector3 forward = leg.forward;
				Vector3 right = leg.right;
				Vector3 footHitNormal = legData.footHitNormal;
				Vector3 val = Vector3.Cross(-right, footHitNormal);
				Vector3 val2 = Vector3.Cross(forward, footHitNormal);
				Vector3 val3 = Vector3.ProjectOnPlane(val, Vector3.get_up());
				Vector3 val4 = Vector3.ProjectOnPlane(val2, Vector3.get_up());
				float num = Vector3.Angle(val, val3);
				float num2 = Vector3.Angle(val2, val4);
				float num3 = Mathf.Sign(Vector3.Dot(right, Vector3.Cross(-forward, val3)));
				float num4 = Mathf.Sign(Vector3.Dot(forward, Vector3.Cross(-right, val4)));
				num *= num3;
				num2 *= num4;
				float num5 = num2;
				Vector3 eulerAngles = leg.get_transform().get_eulerAngles();
				Quaternion val5 = Quaternion.Euler(num5, eulerAngles.y + 90f, num);
				leg.actualFootTransform.set_localRotation(Quaternion.get_identity());
				Quaternion rotation = leg.actualFootTransform.get_rotation();
				Transform actualFootTransform = leg.actualFootTransform;
				Quaternion val6 = val5;
				Quaternion val7 = rotation;
				Vector3 val8 = legData.targetLegPosition - leg.footPosition;
				actualFootTransform.set_rotation(Quaternion.Lerp(val6, val7, Mathf.Abs(val8.y) * Mathf.Sign(leg.legMovement.longJumpForce)));
			}
			else if (leg.legMovement.longJumpForce > 0f)
			{
				leg.actualFootTransform.set_localRotation(Quaternion.get_identity());
			}
		}

		private void UpdateVisibleState(CubeMechLeg leg)
		{
			if (leg.wasHidden != leg.isHidden)
			{
				leg.wasHidden = leg.isHidden;
				leg.ik.set_enabled(!leg.isHidden);
			}
		}

		private void UpdateAnimationState(CubeMechLeg leg)
		{
			MechLegData legData = leg.legData;
			MechLegGraphics legGraphics = leg.legGraphics;
			if (!leg.isHidden && ((legGraphics.canAnimate && !legGraphics.legRetracted) || (legData.legGrounded && legData.legLanding)))
			{
				if (ShouldAnimateFully(leg))
				{
					Animate(leg);
				}
				else if (ShouldAnimateQuietly(leg))
				{
					legGraphics.quietAnimation = true;
					Animate(leg);
				}
			}
		}

		private bool ShouldAnimateFully(CubeMechLeg leg)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			Vector3 val = legData.targetLegPosition - leg.footPosition;
			float sqrMagnitude = val.get_sqrMagnitude();
			return sqrMagnitude > 0.36f && !legData.legLanding && legData.legGrounded;
		}

		private bool ShouldAnimateQuietly(CubeMechLeg leg)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = leg.legData.targetLegPosition - leg.footPosition;
			float sqrMagnitude = val.get_sqrMagnitude();
			return sqrMagnitude > 0.0225f;
		}

		private void Animate(CubeMechLeg leg)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			MechLegGraphics legGraphics = leg.legGraphics;
			if (!legGraphics.isAnimating)
			{
				legGraphics.totalTransitionTime = Mathf.Max(_animationPeriod - _timeTillAnimationUpdate, leg.animationTimeFastStep);
				legGraphics.currentTransitionTime = 0f;
				Vector3 val = legGraphics.startFootPos = Quaternion.Inverse(leg.T.get_rotation()) * (leg.footPosition - leg.position);
			}
			legGraphics.targetFootPos = leg.legData.targetLegPosition;
			legGraphics.isAnimating = true;
		}

		private void UpdateAnimation(CubeMechLeg leg, float deltaTime)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			MechLegData legData = leg.legData;
			if ((legData.terrainSlidingAngleValid && legData.legSliding) || legData.groundedOnItself)
			{
				leg.footPosition = legData.targetLegPosition;
				if (!legData.groundedOnItself)
				{
					_legEffectsManager.PlaySkiddingEffect(leg, leg.footPosition);
				}
				return;
			}
			if (!leg.legEffects.skiddingEffectHidden)
			{
				_legEffectsManager.HideSkiddingEffect(leg);
			}
			MechLegGraphics legGraphics = leg.legGraphics;
			if (legGraphics.isAnimating)
			{
				legGraphics.currentTransitionTime += deltaTime;
				if (legGraphics.currentTransitionTime < legGraphics.totalTransitionTime)
				{
					if (legGraphics.currentTransitionTime + deltaTime > legGraphics.totalTransitionTime)
					{
						AnimationEnded(leg);
					}
					else
					{
						UpdateFootPosAnimation(leg);
					}
				}
				else
				{
					AnimationEnded(leg);
				}
				return;
			}
			legGraphics.currentNonAnimatingTime += deltaTime;
			float num = 0f;
			if (leg.legData.legGrounded)
			{
				float num2 = (!leg.legData.crouching) ? 1 : 0;
				num = Mathf.Lerp(0f, leg.runningAnimationHeight, num2 * leg.legData.currentSpeedRatio);
				num *= leg.footHeightCurve2.Evaluate(legGraphics.currentNonAnimatingTime / _animationPeriod);
			}
			leg.footPosition = legGraphics.targetFootPos + Vector3.get_up() * num;
			leg.hipDirection = legGraphics.stoppedLegForwardVector;
		}

		private void AnimationEnded(CubeMechLeg leg)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			MechLegGraphics legGraphics = leg.legGraphics;
			MechLegData legData = leg.legData;
			legGraphics.canAnimate = false;
			legGraphics.isAnimating = false;
			legGraphics.quietAnimation = false;
			legGraphics.currentTransitionTime = legGraphics.totalTransitionTime;
			legGraphics.currentNonAnimatingTime = 0f;
			legGraphics.stoppedLegForwardVector = -leg.forward;
			legData.legLanding = false;
			if (!legGraphics.justLanded && !legGraphics.justJumped && legData.legGrounded && !legData.legSliding && !legGraphics.quietAnimation)
			{
				_legEffectsManager.PlayWalkingEffect(leg, leg.footPosition);
				if (!legData.crouching)
				{
					_legAudioManager.PlayNormalFootstepSound(leg, _numSyncGroups, _legs.get_Count());
				}
				else
				{
					_legAudioManager.PlayCrouchFootstepSound(leg, _numSyncGroups, _legs.get_Count());
				}
				legGraphics.lastStepSpeed = legData.currentSpeed;
			}
		}

		private void UpdateFootPosAnimation(CubeMechLeg leg)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			MechLegGraphics legGraphics = leg.legGraphics;
			float num = leg.footMovementCurve.Evaluate(legGraphics.animationProgress);
			Vector3 footPosition = leg.footPosition;
			Vector3 val = footPosition;
			Vector3 val2 = (legGraphics.targetFootPos - footPosition) * num;
			Vector3 val3 = Vector3.get_zero();
			if (!legGraphics.quietAnimation && leg.legData.legGrounded)
			{
				float num2 = Mathf.Lerp(0f, leg.animationHeight, leg.legData.currentSpeedRatio);
				val3 = leg.up * num2 * leg.footHeightCurve.Evaluate(legGraphics.animationProgress);
			}
			else if (leg.legData.legLanding)
			{
				val2 = legGraphics.targetFootPos - footPosition;
			}
			leg.footPosition = val + val2 + val3;
			leg.hipDirection = leg.get_transform().get_parent().get_forward();
		}

		private void UpdateNonGroundedState(CubeMechLeg leg, float deltaTime)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			MechLegGraphics legGraphics = leg.legGraphics;
			if (legGraphics.justJumped)
			{
				_legEffectsManager.PlayJumpingEffect(leg, leg.footPosition);
				legGraphics.justJumped = false;
				if (leg.legData.longJumping)
				{
					legGraphics.targetJumpOffset = Vector3.get_down() * Random.Range(-1f, 1f);
					_legEffectsManager.PlayLongJumpEffect(leg, leg.footPosition);
					_legEffectsManager.PlayStartLongJumpEffect(leg, leg.legData.targetLegPosition);
				}
			}
			if (!leg.isHidden && !leg.legData.legGrounded)
			{
				Vector3 zero = Vector3.get_zero();
				if (leg.legData.jumping)
				{
					if (leg.legData.longJumping)
					{
						zero = leg.longJumpingRetractedPositionTransform.get_position() + legGraphics.targetJumpOffset;
						if (legGraphics.justDescending)
						{
							legGraphics.targetJumpOffset = Vector3.get_zero();
							legGraphics.justDescending = false;
							legGraphics.legRetracted = false;
						}
						if (leg.legData.descending)
						{
							zero = leg.longJumpingFallingRetractedPositionTransform.get_position() + legGraphics.targetJumpOffset;
						}
					}
					else
					{
						zero = leg.jumpingRetractedPosition;
					}
				}
				else
				{
					zero = leg.fallingRetractedPosition;
				}
				if (!legGraphics.legRetracted)
				{
					if (leg.legData.longJumping)
					{
						legGraphics.totalTransitionTime = leg.animationTimeSlowStep;
						legGraphics.currentTransitionTime = 0f;
					}
					else
					{
						legGraphics.currentTransitionTime = 0f;
						legGraphics.totalTransitionTime = leg.animationTimeFastStep * 0.5f;
					}
					Vector3 val = legGraphics.startFootPos = Quaternion.Inverse(leg.T.get_rotation()) * (leg.footPosition - leg.position);
					legGraphics.targetFootPos = zero;
					legGraphics.isAnimating = true;
					legGraphics.legRetracted = true;
				}
				else if (legGraphics.isAnimating)
				{
					legGraphics.targetFootPos = zero;
				}
				else
				{
					leg.footPosition = zero;
					legGraphics.targetFootPos = zero;
				}
			}
			else
			{
				if (legGraphics.justLanded)
				{
					_legEffectsManager.PlayLandingEffect(leg, leg.legData.targetLegPosition);
					legGraphics.justLanded = false;
				}
				legGraphics.legRetracted = false;
			}
		}
	}
}
