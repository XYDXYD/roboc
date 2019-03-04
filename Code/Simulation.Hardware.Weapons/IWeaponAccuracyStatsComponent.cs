namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponAccuracyStatsComponent
	{
		float baseInAccuracyDegrees
		{
			get;
			set;
		}

		float baseAirInaccuracyDegrees
		{
			get;
			set;
		}

		float movementInAccuracyDegrees
		{
			get;
			set;
		}

		float movementMaxThresholdSpeed
		{
			get;
			set;
		}

		float movementMinThresholdSpeed
		{
			get;
			set;
		}

		float gunRotationThresholdSlow
		{
			get;
			set;
		}

		float movementInAccuracyDecayTime
		{
			get;
			set;
		}

		float slowRotationInAccuracyDecayTime
		{
			get;
			set;
		}

		float quickRotationInAccuracyDecayTime
		{
			get;
			set;
		}

		float movementInAccuracyRecoveryTime
		{
			get;
			set;
		}

		float repeatFireInAccuracyTotalDegrees
		{
			get;
			set;
		}

		float repeatFireInAccuracyDecayTime
		{
			get;
			set;
		}

		float repeatFireInAccuracyRecoveryTime
		{
			get;
			set;
		}

		float fireInstantAccuracyDecayDegrees
		{
			get;
			set;
		}

		float accuracyNonRecoverTime
		{
			get;
			set;
		}

		float accuracyDecayTime
		{
			get;
			set;
		}
	}
}
