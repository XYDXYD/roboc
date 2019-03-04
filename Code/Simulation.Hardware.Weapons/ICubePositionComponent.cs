using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface ICubePositionComponent
	{
		Vector3 position
		{
			get;
		}

		Byte3 gridPos
		{
			get;
		}
	}
}
