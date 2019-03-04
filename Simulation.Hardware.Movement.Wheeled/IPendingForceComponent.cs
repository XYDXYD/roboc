using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IPendingForceComponent
	{
		Vector3 pendingForce
		{
			get;
			set;
		}

		Vector3 pendingVelocityChangeForce
		{
			get;
			set;
		}
	}
}
