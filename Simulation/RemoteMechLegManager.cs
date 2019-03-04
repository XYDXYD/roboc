using UnityEngine;

namespace Simulation
{
	internal sealed class RemoteMechLegManager : MechLegManager
	{
		private IMachineControl _inputWrapper;

		public RemoteMechLegManager(Rigidbody rb, NetworkMachineManager machineManager, IMachineControl inputWrapper)
			: base(rb, machineManager, localPlayer: false)
		{
			_inputWrapper = inputWrapper;
		}

		public override void Tick(float deltaTime)
		{
			if (_legs.Count > 0)
			{
				DecideIfJumping();
				DecideIfCrouching();
				ProcessJumping(deltaTime);
				UpdateGraphics(deltaTime);
			}
		}

		private void DecideIfCrouching()
		{
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeMechLeg cubeMechLeg = _legs[i];
				MechLegData legData = cubeMechLeg.legData;
				if (_inputWrapper.moveDown && legData.legGrounded)
				{
					legData.targetHeight = cubeMechLeg.movement.idealCrouchingHeight;
					legData.crouching = true;
				}
				else
				{
					legData.targetHeight = cubeMechLeg.movement.idealHeight;
					legData.crouching = false;
				}
			}
		}

		public override void PhysicsTick(float deltaTime)
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			if (_legs.Count <= 0)
			{
				return;
			}
			ProcessLegMass();
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeMechLeg cubeMechLeg = _legs[i];
				if (!(cubeMechLeg.T == null))
				{
					if (!cubeMechLeg.isHidden)
					{
						cubeMechLeg.UpdateCachedValues();
						CheckIsDescending(cubeMechLeg);
						ProcessCurrentVelocity(cubeMechLeg, deltaTime);
						UpdateGroundedOnItself(cubeMechLeg);
						UpdateVerticalGroundPosition(cubeMechLeg, deltaTime);
						UpdateTargetGroundedPosition(cubeMechLeg, deltaTime);
					}
					else
					{
						cubeMechLeg.UpdateCachedValues();
						UpdateVerticalGroundPosition(cubeMechLeg, deltaTime);
						cubeMechLeg.footPosition = cubeMechLeg.legData.targetLegPosition;
					}
				}
			}
		}

		private void ProcessJumping(float deltaTime)
		{
			_jumpTimeRemaining -= deltaTime;
			if (!(_jumpTimeRemaining <= 0f))
			{
				return;
			}
			for (int i = 0; i < _legs.Count; i++)
			{
				MechLegData legData = _legs[i].legData;
				if (legData.legGrounded)
				{
					legData.jumping = false;
				}
			}
		}

		private void DecideIfJumping()
		{
			if (_inputWrapper.moveUpwards && !_justJumped)
			{
				for (int i = 0; i < _legs.Count; i++)
				{
					MechLegData legData = _legs[i].legData;
					if (legData.jumping || legData.legSliding)
					{
						return;
					}
				}
				_justJumped = true;
				_jumpTimeRemaining = _jumpDuration;
				bool flag = CheckLongJump();
				for (int j = 0; j < _legs.Count; j++)
				{
					CubeMechLeg cubeMechLeg = _legs[j];
					cubeMechLeg.legData.jumping = true;
					cubeMechLeg.legGraphics.justJumped = true;
					cubeMechLeg.legData.longJumping = (flag && cubeMechLeg.legMovement.longJumpForce > 0f);
				}
			}
			else
			{
				_justJumped = false;
			}
		}

		private bool CheckLongJump()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			float num = 1f;
			float num2 = 0f;
			Vector3 val = Vector3.get_zero();
			for (int i = 0; i < _legs.Count; i++)
			{
				CubeMechLeg cubeMechLeg = _legs[i];
				num *= cubeMechLeg.movement.longJumpForce;
				num2 += cubeMechLeg.legData.currentSpeedRatio;
				val += cubeMechLeg.legData.currentVelocity;
			}
			num2 /= (float)_legs.Count;
			bool flag = num > 0f && num2 > 0.8f;
			if (flag)
			{
				val.y = 0f;
				val.Normalize();
				Vector3 val2 = Vector3.Cross(_rb.get_transform().get_right(), Vector3.get_up());
				if (Vector3.Dot(val, val2) < 0.5f)
				{
					flag = false;
				}
			}
			return flag;
		}
	}
}
