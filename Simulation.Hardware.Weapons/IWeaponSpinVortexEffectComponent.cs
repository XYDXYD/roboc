using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	public interface IWeaponSpinVortexEffectComponent
	{
		Renderer spinVortexRenderer
		{
			get;
		}

		float spinVortexThreshold
		{
			get;
		}
	}
}
