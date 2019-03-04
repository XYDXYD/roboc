using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileEffectExplosionComponent
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
