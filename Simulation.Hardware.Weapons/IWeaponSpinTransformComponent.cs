using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	public interface IWeaponSpinTransformComponent
	{
		Transform spinBarrelTransform
		{
			get;
		}

		float spinBarrelSpeedScale
		{
			get;
		}
	}
}
