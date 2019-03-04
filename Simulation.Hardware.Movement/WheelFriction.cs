using System;
using UnityEngine;

namespace Simulation.Hardware.Movement
{
	[Serializable]
	internal sealed class WheelFriction
	{
		public float movingRPMRatio = 0.05f;

		public InterpollationCurve stoppedCurve = InterpollationCurve.InverseSquare;

		public float F_ExtremumSlip = 0.4f;

		public float F_ExtremumValue = 1f;

		public float F_AsymptoteSlip = 0.8f;

		public float F_AsymptoteValue = 0.5f;

		public float F_StiffnessFactorStopped = 1.2f;

		public float F_StiffnessFactorMoving = 0.8f;

		public float S_ExtremumSlip = 0.2f;

		public float S_ExtremumValue = 1f;

		public float S_AsymptoteSlip = 0.5f;

		public float S_AsymptoteValue = 0.75f;

		public float S_StiffnessFactorStopped = 1f;

		public float S_StiffnessFactorMoving = 3f;

		public float groundFrictionMultiplier = 1f;

		public WheelFrictionCurve GetForwardWheelFrictionCurve(float friction, float rpmRatio)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			WheelFrictionCurve result = default(WheelFrictionCurve);
			float num = F_StiffnessFactorMoving;
			if (rpmRatio < movingRPMRatio)
			{
				float curve = InterpollationCurves.GetCurve(rpmRatio, movingRPMRatio, stoppedCurve);
				num = F_StiffnessFactorMoving + curve * (F_StiffnessFactorStopped - F_StiffnessFactorMoving);
			}
			result.set_extremumSlip(F_ExtremumSlip);
			result.set_extremumValue(F_ExtremumValue);
			result.set_asymptoteSlip(F_AsymptoteSlip);
			result.set_asymptoteValue(F_AsymptoteValue);
			result.set_stiffness(num * groundFrictionMultiplier * friction);
			return result;
		}

		public WheelFrictionCurve GetSidewaysWheelFrictionCurve(float friction, float rpmRatio)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			WheelFrictionCurve result = default(WheelFrictionCurve);
			float num = (!(rpmRatio < 0.01f)) ? S_StiffnessFactorMoving : S_StiffnessFactorStopped;
			result.set_extremumSlip(S_ExtremumSlip);
			result.set_extremumValue(S_ExtremumValue);
			result.set_asymptoteSlip(S_AsymptoteSlip);
			result.set_asymptoteValue(S_AsymptoteValue);
			result.set_stiffness(num * groundFrictionMultiplier * friction);
			return result;
		}
	}
}
