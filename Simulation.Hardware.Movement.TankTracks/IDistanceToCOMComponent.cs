using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface IDistanceToCOMComponent
	{
		Vector3 peviousCentreOfMass
		{
			get;
			set;
		}

		float distance
		{
			get;
			set;
		}
	}
}
