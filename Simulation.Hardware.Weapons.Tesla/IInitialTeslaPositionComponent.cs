using UnityEngine;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal interface IInitialTeslaPositionComponent
	{
		Vector3 initialPosition
		{
			get;
		}

		Quaternion initialRotation
		{
			get;
		}
	}
}
