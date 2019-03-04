using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IMachineTiltComponent
	{
		Vector3 localMovementTilt
		{
			get;
			set;
		}

		Vector3 targetLocalTilt
		{
			get;
			set;
		}

		Vector3 localBalanceTilt
		{
			get;
			set;
		}
	}
}
