namespace Simulation
{
	internal sealed class MechLegMovement
	{
		private MechLegData _legData;

		private MechLegMovementData _legMovement;

		public float heightMultiplier
		{
			get;
			set;
		}

		public float idealHeight => _legMovement.idealHeight;

		public float idealCrouchingHeight => _legMovement.idealCrouchingHeight;

		public float idealHeightRange => _legMovement.idealHeightRange;

		public float jumpHeight => _legMovement.jumpHeight * heightMultiplier;

		public float longJumpForce => _legMovement.longJumpForce;

		public float idealSlideHeight => _legMovement.idealSlideHeight;

		public float maxUpwardsForce => _legData.massPerLeg * _legMovement.maxUpwardsForce;

		public float maxBobForce => _legData.massPerLeg * _legMovement.maxBobForce;

		public float maxLateralForce => _legData.massPerLeg * _legMovement.maxLateralForce;

		public float maxDampingForce => _legData.massPerLeg * _legMovement.maxDampingForce;

		public float maxStoppedForce => _legData.massPerLeg * _legMovement.maxStoppedForce;

		public float upwardsDampingForce => _legData.massPerLeg * _legMovement.upwardsDampingForce;

		public float lateralDampForce => _legData.massPerLeg * _legMovement.lateralDampForce;

		public float swaggerForce => _legData.massPerLeg * _legMovement.swaggerForce;

		public float maxLegacyTurnRate => _legMovement.maxLegacyTurnRate;

		public float legacyTurnAcceleration => _legMovement.legacyTurnAcceleration;

		public float maxTurnRate => _legMovement.maxTurnRate;

		public float turnAcceleration => _legMovement.turnAcceleration;

		public float turningDrift => _legMovement.turningDrift;

		public float maxBobHeight => _legMovement.maxBobHeight;

		public float inAirTurnAmount => _legMovement.inAirTurnAmount;

		public float crouchingSpeedScale => _legMovement.crouchingSpeedScale;

		public float newTurnDampingScale => _legMovement.newTurnDampingScale;

		public float legacyTurnDampingScale => _legMovement.legacyTurnDampingScale;

		public float stoppingTurnDampingScale => _legMovement.stoppingTurnDampingScale;

		public MechLegMovement(CubeMechLeg leg)
		{
			_legData = leg.legData;
			_legMovement = leg.legMovement;
			heightMultiplier = 1f;
		}
	}
}
