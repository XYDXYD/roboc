using UnityEngine;

namespace Simulation.Hardware.Weapons.RailGun
{
	internal interface IRailProjectileTrailComponent
	{
		GameObject projectileMesh
		{
			get;
		}

		GameObject projectileGlow
		{
			get;
		}

		TrailRenderer projectileTrail
		{
			get;
		}

		Transform smoke
		{
			get;
		}

		Material smokeMaterial
		{
			get;
		}

		float textureScaleMultiplier
		{
			get;
		}

		float tilingScale
		{
			get;
		}

		float originalTextureScale
		{
			get;
		}

		Vector3 originalSmokeScale
		{
			get;
		}

		float smokeFadeDelay
		{
			get;
		}

		float currentSmokeFadeAmount
		{
			get;
			set;
		}

		bool allowFadeSmoke
		{
			get;
			set;
		}

		float smokeFadeUnitsPerSecond
		{
			get;
		}

		float smokeRotateRate
		{
			get;
		}

		float smokeDiameterScaleRate
		{
			get;
		}

		float originalBeamCollapseTime
		{
			get;
		}

		float beamCollapseTimeMultiplier
		{
			get;
		}
	}
}
