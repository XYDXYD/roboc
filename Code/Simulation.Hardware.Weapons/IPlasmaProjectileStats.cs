namespace Simulation.Hardware.Weapons
{
	internal interface IPlasmaProjectileStats
	{
		float plasmaExplosionRadius
		{
			get;
			set;
		}

		float plasmaDamageAtEdge
		{
			get;
			set;
		}

		float plasmaTimeToFullDamage
		{
			get;
			set;
		}

		float plasmaStartingRadiusScale
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
