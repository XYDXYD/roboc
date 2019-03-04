using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	public interface IWeaponHeatEffectComponent
	{
		Renderer[] heatRenderers
		{
			get;
		}

		float heatIncreaseSpeed
		{
			get;
		}

		float heatDecreaseSpeed
		{
			get;
		}

		float currentHeat
		{
			get;
			set;
		}
	}
}
