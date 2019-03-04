using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileEffectImpactEnvironmentComponent
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
