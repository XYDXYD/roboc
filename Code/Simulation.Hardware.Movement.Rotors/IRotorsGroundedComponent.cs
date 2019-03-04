using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IRotorsGroundedComponent
	{
		bool grounded
		{
			get;
			set;
		}

		bool prevDescending
		{
			get;
			set;
		}

		Vector3 prevPosition
		{
			get;
			set;
		}
	}
}
