using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileEffectImpactFusionShieldComponent
	{
		GameObject prefab
		{
			get;
		}

		GameObject prefab_E
		{
			get;
		}

		string audioEvent
		{
			get;
		}
	}
}
