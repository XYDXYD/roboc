using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal interface IThrusterForceAppliedComponent
	{
		bool forceApplied
		{
			get;
			set;
		}

		Vector3 forceDirection
		{
			get;
			set;
		}
	}
}
