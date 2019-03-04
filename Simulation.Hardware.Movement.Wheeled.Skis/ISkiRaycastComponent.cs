using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal interface ISkiRaycastComponent
	{
		Transform raycastPoint
		{
			get;
		}

		float raycastOffset
		{
			get;
		}

		float raycastDist
		{
			get;
		}

		float raycastBackTrack
		{
			get;
		}
	}
}
