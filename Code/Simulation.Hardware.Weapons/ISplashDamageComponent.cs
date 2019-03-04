namespace Simulation.Hardware.Weapons
{
	internal interface ISplashDamageComponent
	{
		float damageRadius
		{
			get;
			set;
		}

		float coneAngle
		{
			get;
		}

		int additionalHits
		{
			get;
		}
	}
}
