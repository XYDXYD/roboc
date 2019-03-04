using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponMovementComponent
	{
		Vector3 velocity
		{
			get;
			set;
		}

		Vector3 rotationVelocity
		{
			get;
			set;
		}

		Vector3 lastPosition
		{
			get;
			set;
		}

		Vector3 lastRotation
		{
			get;
			set;
		}
	}
}
