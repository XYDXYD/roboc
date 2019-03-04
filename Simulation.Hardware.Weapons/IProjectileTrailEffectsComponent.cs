using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileTrailEffectsComponent
	{
		GameObject trailRendererObject
		{
			get;
		}

		float disableCountdown
		{
			get;
			set;
		}

		TrailRenderer projectileTrail
		{
			get;
		}

		GameObject disableOnHit
		{
			get;
		}
	}
}
