using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled
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
	}
}
