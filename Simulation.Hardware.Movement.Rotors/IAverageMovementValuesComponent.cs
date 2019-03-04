using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IAverageMovementValuesComponent
	{
		float avgCeilingHeightModifier
		{
			get;
			set;
		}

		float maxCarryingMass
		{
			get;
			set;
		}

		float avgMaxHeightChangeSpeed
		{
			get;
			set;
		}

		float avgHeightAcceleration
		{
			get;
			set;
		}

		float avgStrafeAcceleration
		{
			get;
			set;
		}

		float avgTurnAcceleration
		{
			get;
			set;
		}

		float avgTurnMaxRate
		{
			get;
			set;
		}

		float avgTurnTangentalAcceleration
		{
			get;
			set;
		}

		float avgTurnTangentalMaxSpeed
		{
			get;
			set;
		}

		float avgLevelAcceleration
		{
			get;
			set;
		}

		float avgLevelRate
		{
			get;
			set;
		}

		float avgDriftAcceleration
		{
			get;
			set;
		}

		float avgDriveMaxSpeed
		{
			get;
			set;
		}

		float avgDriftMaxSpeedAngle
		{
			get;
			set;
		}

		float avgRotorSize
		{
			get;
			set;
		}

		float avgZeroTiltSize
		{
			get;
			set;
		}

		float avgTilt
		{
			get;
			set;
		}

		float avgMovementTilt
		{
			get;
			set;
		}

		float avgBankTilt
		{
			get;
			set;
		}

		float avgFullHoverAngle
		{
			get;
			set;
		}

		float avgMinHoverAngle
		{
			get;
			set;
		}

		float avgMinHoverRatio
		{
			get;
			set;
		}

		float avgHoverRadiusSqr
		{
			get;
			set;
		}

		Vector3 avgForcePos
		{
			get;
			set;
		}
	}
}
