using System;
using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	[Serializable]
	internal sealed class TankTrackFriction
	{
		public float F_ExtremumSlip = 1f;

		public float F_ExtremumValue = 20000f;

		public float F_AsymptoteSlip = 2f;

		public float F_AsymptoteValue = 10000f;

		public float F_StiffnessFactorStopped = 0.1f;

		public float F_StiffnessFactorMoving = 0.1f;

		public float F_StiffnessFactor = 0.1f;

		public float S_ExtremumSlip = 1f;

		public float S_ExtremumValue = 20000f;

		public float S_AsymptoteSlip = 2f;

		public float S_AsymptoteValue = 10000f;

		public float S_StiffnessFactorStopped = 0.1f;

		public float S_StiffnessFactorMoving = 0.1f;

		public float S_StiffnessFactor = 0.1f;

		public WheelFrictionCurve GetForwardWheelFrictionCurve()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			WheelFrictionCurve result = default(WheelFrictionCurve);
			result.set_extremumSlip(F_ExtremumSlip);
			result.set_extremumValue(F_ExtremumValue);
			result.set_asymptoteSlip(F_AsymptoteSlip);
			result.set_asymptoteValue(F_AsymptoteValue);
			result.set_stiffness(F_StiffnessFactor);
			return result;
		}

		public WheelFrictionCurve GetSidewaysWheelFrictionCurve()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			WheelFrictionCurve result = default(WheelFrictionCurve);
			result.set_extremumSlip(S_ExtremumSlip);
			result.set_extremumValue(S_ExtremumValue);
			result.set_asymptoteSlip(S_AsymptoteSlip);
			result.set_asymptoteValue(S_AsymptoteValue);
			result.set_stiffness(S_StiffnessFactor);
			return result;
		}
	}
}
