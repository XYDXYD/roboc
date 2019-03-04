using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IMachineDriftComponent
	{
		Vector3 targetLocalDrift
		{
			get;
			set;
		}

		float targetDriftSpeed
		{
			get;
			set;
		}
	}
}
