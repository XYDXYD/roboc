using System.Collections.Generic;

namespace Simulation.Hardware.Weapons
{
	internal interface IWeaponShotDirectionsComponent
	{
		List<float> directionAngles
		{
			get;
		}

		bool multipleShot
		{
			get;
		}
	}
}
