using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface ICameraShakeDamageComponent
	{
		Transform transformToShake
		{
			get;
		}

		int maxSimultaneouslyShake
		{
			get;
		}

		float rotationDamageMagnitudeMultiplier
		{
			get;
		}

		float translationDamageMagnitudeMultiplier
		{
			get;
		}

		float damageDuration
		{
			get;
		}

		TranslationCurve translationDamageCurves
		{
			get;
		}

		RotationCurve rotationDamageCurves
		{
			get;
		}
	}
}
