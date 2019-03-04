using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IGroundedComponent
	{
		bool grounded
		{
			get;
			set;
		}

		float distanceToGround
		{
			get;
			set;
		}

		Vector3 hitNormal
		{
			get;
			set;
		}
	}
}
