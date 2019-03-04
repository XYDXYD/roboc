using System;

namespace Simulation
{
	[Serializable]
	internal sealed class LegMovementData
	{
		public float idealHeight = 1f;

		public float idealCrouchingHeight = 0.4f;

		public float idealHeightRange = 0.3f;

		public float jumpHeight = 2f;

		public float maxUpwardsForce = 25f;

		public float maxLateralForce = 25f;

		public float maxTurningForce = 20f;

		public float maxDampingForce = 10f;

		public float maxStoppedForce = 60f;

		public float maxNewStoppedForce = 10f;

		public float upwardsDampingForce = 10f;

		public float lateralDampForce = 10f;

		public float swaggerForce = 0.1f;
	}
}
