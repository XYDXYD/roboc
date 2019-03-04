using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal interface ISkiSuspensionComponent
	{
		Vector3 compressedOffset
		{
			get;
		}

		Vector3 extendedOffset
		{
			get;
		}

		float travel
		{
			get;
			set;
		}

		Vector3 localPosition
		{
			get;
			set;
		}
	}
}
