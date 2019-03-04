using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileEffectImpactMissComponent
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
