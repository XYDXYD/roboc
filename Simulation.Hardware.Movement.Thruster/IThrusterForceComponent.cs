using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal interface IThrusterForceComponent
	{
		float force
		{
			get;
		}

		Vector3 direction
		{
			get;
		}
	}
}
