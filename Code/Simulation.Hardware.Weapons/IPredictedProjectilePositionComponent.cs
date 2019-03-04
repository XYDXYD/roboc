using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IPredictedProjectilePositionComponent
	{
		Vector3 currentPos
		{
			get;
			set;
		}

		Vector3 nextPos
		{
			get;
			set;
		}
	}
}
