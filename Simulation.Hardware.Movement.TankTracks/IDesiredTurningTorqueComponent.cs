using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface IDesiredTurningTorqueComponent
	{
		Vector3 desiredTorque
		{
			get;
			set;
		}
	}
}
