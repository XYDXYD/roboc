namespace Simulation.Hardware.Weapons
{
	internal interface IRobotShakeDamageComponent
	{
		int maxSimultaneouslyShake
		{
			get;
		}

		float damageMagnitudeMultiplier
		{
			get;
		}

		float damageDuration
		{
			get;
		}

		TranslationCurve damageCurves
		{
			get;
		}

		float minimumMagnitude
		{
			get;
		}
	}
}
