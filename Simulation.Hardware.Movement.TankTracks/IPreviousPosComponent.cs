using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface IPreviousPosComponent
	{
		Vector3 previousPos
		{
			get;
			set;
		}
	}
}
