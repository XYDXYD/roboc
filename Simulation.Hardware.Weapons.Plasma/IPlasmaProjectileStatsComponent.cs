namespace Simulation.Hardware.Weapons.Plasma
{
	internal interface IPlasmaProjectileStatsComponent
	{
		float explosionRadius
		{
			get;
			set;
		}

		float timeToFullDamage
		{
			get;
			set;
		}

		float startingRadiusScale
		{
			get;
			set;
		}

		float currentExplosionRadius
		{
			get;
			set;
		}
	}
}
