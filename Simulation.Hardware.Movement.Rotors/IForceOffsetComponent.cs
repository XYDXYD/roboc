using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IForceOffsetComponent
	{
		Vector3 localForceOffset
		{
			get;
			set;
		}
	}
}
