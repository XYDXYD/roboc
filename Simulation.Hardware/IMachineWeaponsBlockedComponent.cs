namespace Simulation.Hardware
{
	internal interface IMachineWeaponsBlockedComponent
	{
		bool blocked
		{
			get;
			set;
		}

		bool blockedByFusionShield
		{
			get;
			set;
		}

		bool weaponNotGrounded
		{
			get;
			set;
		}

		bool lastWeaponShotBlocked
		{
			get;
			set;
		}
	}
}
