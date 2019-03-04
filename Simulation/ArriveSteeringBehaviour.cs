using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class ArriveSteeringBehaviour
	{
		private const float TAN_10_DEG = 0.176326975f;

		private float _Kp;

		private float _maxSpeed;

		private Vector3 _desiredPosition;

		private float _maxAcceleration;

		public event Action OnArrived = delegate
		{
		};

		public ArriveSteeringBehaviour(float maxSpeed, float transitionTimeParameter, float fixedUpdateDt)
		{
			_maxSpeed = maxSpeed;
			_Kp = (1f - Mathf.Pow(0.01f, fixedUpdateDt / transitionTimeParameter)) / fixedUpdateDt;
			_maxAcceleration = _maxSpeed * _Kp;
		}

		public float GetResponsivenessConstant()
		{
			return _Kp;
		}

		public void SetMaxSpeed(float maxSpeed)
		{
			_maxSpeed = maxSpeed;
			_maxAcceleration = _maxSpeed * _Kp;
		}

		public float GetMaxSpeed()
		{
			return _maxSpeed;
		}

		public void SetDesiredPosition(Vector3 desiredPosition)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_desiredPosition = desiredPosition;
		}

		public Vector3 FixedUpdate(Vector3 desiredVelocity, Vector3 curVelocity, float dt)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			if (desiredVelocity.get_sqrMagnitude() > curVelocity.get_sqrMagnitude())
			{
				if (desiredVelocity.get_sqrMagnitude() > _maxSpeed * _maxSpeed)
				{
					desiredVelocity.Normalize();
					desiredVelocity *= _maxSpeed;
				}
				Vector3 val = (curVelocity - desiredVelocity) * (0f - _Kp);
				desiredVelocity = curVelocity + val * dt;
			}
			else
			{
				Vector3 val2 = (desiredVelocity - curVelocity) / dt;
				if (val2.get_sqrMagnitude() > _maxAcceleration * _maxAcceleration)
				{
					val2.Normalize();
					val2 *= _maxAcceleration;
					desiredVelocity = curVelocity + val2 * dt;
				}
			}
			if (curVelocity.get_sqrMagnitude() < _maxSpeed * 0.01f * (_maxSpeed * 0.01f))
			{
				Vector3 val3 = desiredVelocity - curVelocity;
				if (val3.get_sqrMagnitude() / (dt * dt) < 0.0310912021f)
				{
					this.OnArrived();
				}
			}
			return desiredVelocity;
		}

		public Vector3 ComputeDesiredVelocity(Vector3 curPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = curPosition - _desiredPosition;
			return val * (0f - _Kp);
		}
	}
}
