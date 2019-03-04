using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileEffectImpactSelfComponent
	{
		GameObject prefab
		{
			get;
		}

		string audioEvent
		{
			get;
		}
	}
}
