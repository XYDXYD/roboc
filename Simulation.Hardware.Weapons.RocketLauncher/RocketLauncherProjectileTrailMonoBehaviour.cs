using UnityEngine;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class RocketLauncherProjectileTrailMonoBehaviour : MonoBehaviour, IRocketLauncherProjectileTrailEffects
	{
		public TrailRenderer trailRenderer;

		public GameObject projectile;

		public float maxTrailTime = 3.5f;

		GameObject IRocketLauncherProjectileTrailEffects.projectile
		{
			get
			{
				return projectile;
			}
		}

		TrailRenderer IRocketLauncherProjectileTrailEffects.trailRenderer
		{
			get
			{
				return trailRenderer;
			}
		}

		float IRocketLauncherProjectileTrailEffects.disableCountdown
		{
			get;
			set;
		}

		float IRocketLauncherProjectileTrailEffects.maxTrailTime
		{
			get
			{
				return maxTrailTime;
			}
		}

		bool IRocketLauncherProjectileTrailEffects.trailResetNeeded
		{
			get;
			set;
		}

		public RocketLauncherProjectileTrailMonoBehaviour()
			: this()
		{
		}
	}
}
