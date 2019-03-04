using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IHomingProjectileStatsComponent
	{
		Vector3 angularVelocity
		{
			get;
			set;
		}

		float maxRotationAccelerationRad
		{
			get;
			set;
		}

		float maxRotationSpeedRad
		{
			get;
			set;
		}

		float initialRotationSpeedRad
		{
			get;
			set;
		}
	}
}
