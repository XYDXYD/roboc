using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileMovementStatsComponent
	{
		Vector3 startDirection
		{
			get;
			set;
		}

		Vector3 startPosition
		{
			get;
			set;
		}

		Vector3 startVelocity
		{
			get;
			set;
		}

		float speed
		{
			get;
			set;
		}

		Vector3 buildPosition
		{
			get;
			set;
		}

		Vector3 robotStartPosition
		{
			get;
			set;
		}
	}
}
