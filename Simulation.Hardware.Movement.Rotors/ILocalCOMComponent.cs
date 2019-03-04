using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal interface ILocalCOMComponent
	{
		Vector3 localCOM
		{
			get;
			set;
		}
	}
}
