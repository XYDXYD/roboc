using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class RotationArriveSteeringBehaviour
	{
		public const float TAN_5_DEG = 0.0874886662f;

		private const float SIN_1_DEG = 0.0174524058f;

		private const float TAN_10_DEG = 0.176326975f;

		private float _Kpr;

		private float _maxAngularVelocity;

		private float _maxAngAcc;

		private Vector3 _desiredUp;

		public event Action OnArrived = delegate
		{
		};

		public RotationArriveSteeringBehaviour(float maxAngularVelocity, float transitionTimeParameter, float fixedUpdateDt)
		{
			_maxAngularVelocity = maxAngularVelocity;
			_Kpr = (1f - Mathf.Pow(0.01f, fixedUpdateDt / transitionTimeParameter)) / fixedUpdateDt * (float)Math.PI * 0.5f;
			_maxAngAcc = _maxAngularVelocity * _Kpr;
		}

		public float GetMaxAngularVelocity()
		{
			return _maxAngularVelocity;
		}

		public void SetMaxAngularVelocity(float maxAngularVelocity)
		{
			_maxAngularVelocity = maxAngularVelocity;
			_maxAngAcc = _maxAngularVelocity * _Kpr;
		}

		public void SetDesiredUp(Vector3 desiredUp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_desiredUp = desiredUp;
		}

		public Vector3 FixedUpdate(Vector3 desiredAngVel, Vector3 curAngularVel, float dt)
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
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
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
			if (desiredAngVel.get_sqrMagnitude() > curAngularVel.get_sqrMagnitude())
			{
				if (desiredAngVel.get_sqrMagnitude() > _maxAngularVelocity * _maxAngularVelocity)
				{
					desiredAngVel.Normalize();
					desiredAngVel *= _maxAngularVelocity;
				}
				Vector3 val = (curAngularVel - desiredAngVel) * (0f - _Kpr);
				desiredAngVel = val * dt + curAngularVel;
			}
			else
			{
				Vector3 val2 = (desiredAngVel - curAngularVel) / dt;
				if (val2.get_sqrMagnitude() > _maxAngAcc * _maxAngAcc)
				{
					val2.Normalize();
					val2 *= _maxAngAcc;
					desiredAngVel = curAngularVel + val2 * dt;
				}
			}
			if (curAngularVel.get_sqrMagnitude() < _maxAngularVelocity * 0.01f * _maxAngularVelocity * 0.01f)
			{
				Vector3 val3 = desiredAngVel - curAngularVel;
				if (val3.get_sqrMagnitude() / (dt * dt) < 0.0310912021f)
				{
					this.OnArrived();
				}
			}
			return desiredAngVel;
		}

		public Vector3 ComputeDesiredAngularVelocity(Vector3 currentUp, Vector3 fallbackRotationAxis)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.Cross(_desiredUp, currentUp);
			if (Vector3.Dot(_desiredUp, currentUp) < 0f)
			{
				if (val.get_sqrMagnitude() > 0.000304586458f)
				{
					val.Normalize();
				}
				else
				{
					val = fallbackRotationAxis;
				}
			}
			return val * (0f - _Kpr);
		}
	}
}
