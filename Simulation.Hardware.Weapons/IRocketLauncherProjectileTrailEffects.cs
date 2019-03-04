using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IRocketLauncherProjectileTrailEffects
	{
		GameObject projectile
		{
			get;
		}

		TrailRenderer trailRenderer
		{
			get;
		}

		float disableCountdown
		{
			get;
			set;
		}

		float maxTrailTime
		{
			get;
		}

		bool trailResetNeeded
		{
			get;
			set;
		}
	}
}
