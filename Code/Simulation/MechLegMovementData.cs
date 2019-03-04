using System;

namespace Simulation
{
	[Serializable]
	internal sealed class MechLegMovementData
	{
		public float idealHeight = 1f;

		public float idealCrouchingHeight = 0.4f;

		public float idealHeightRange = 0.3f;

		public float jumpHeight = 2f;

		public float longJumpForce;

		public float idealSlideHeight = 2.2f;

		public float maxUpwardsForce = 25f;

		public float maxBobForce = 25f;

		public float maxLateralForce = 20f;

		public float maxDampingForce = 10f;

		public float maxStoppedForce = 60f;

		public float upwardsDampingForce = 10f;

		public float lateralDampForce = 10f;

		public float swaggerForce = 0.1f;

		public float maxLateralSpeed = 8f;

		public float turnAcceleration = 15f;

		public float maxTurnRate = 25f;

		public float maxLegacyTurnRate = 6f;

		public float legacyTurnAcceleration = 4f;

		public float turningDrift = 0.5f;

		public float maxBobHeight = 0.4f;

		public float inAirTurnAmount = 0.8f;

		public float crouchingSpeedScale = 0.75f;

		public float longJumpSpeedScale = 1.5f;

		public float newTurnDampingScale = 0.5f;

		public float legacyTurnDampingScale = 0.5f;

		public float stoppingTurnDampingScale = 0.1f;
	}
}
