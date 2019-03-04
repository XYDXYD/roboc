using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IPreviousStateComponent
	{
		float prevSpeed
		{
			get;
			set;
		}

		Vector3 prevPosition
		{
			get;
			set;
		}

		Quaternion prevRotation
		{
			get;
			set;
		}
	}
}
