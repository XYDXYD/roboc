using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal interface ISkiHingeComponent
	{
		Quaternion currentRotation
		{
			get;
			set;
		}
	}
}
