using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface IForcePointComponent
	{
		Transform forcePointTransform
		{
			get;
		}

		Vector3 forcePoint
		{
			get;
			set;
		}

		Vector3 forcePointOffset
		{
			get;
			set;
		}
	}
}
