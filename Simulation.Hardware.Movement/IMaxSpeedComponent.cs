using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal interface IMaxSpeedComponent
	{
		Vector3 positiveAxisMaxSpeed
		{
			get;
			set;
		}

		Vector3 negativeAxisMaxSpeed
		{
			get;
			set;
		}

		float maxSpeed
		{
			get;
			set;
		}
	}
}
