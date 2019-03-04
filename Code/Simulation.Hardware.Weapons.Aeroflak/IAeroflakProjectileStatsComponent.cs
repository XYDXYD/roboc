namespace Simulation.Hardware.Weapons.AeroFlak
{
	internal interface IAeroflakProjectileStatsComponent
	{
		int damageProximityHit
		{
			get;
			set;
		}

		float damageRadius
		{
			get;
			set;
		}

		float explosionRadius
		{
			get;
			set;
		}

		float groundClearance
		{
			get;
			set;
		}
	}
}
