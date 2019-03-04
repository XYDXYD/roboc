using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface IFastSideTurningTorqueComponent
	{
		Vector3 fastSideTorque
		{
			get;
			set;
		}
	}
}
