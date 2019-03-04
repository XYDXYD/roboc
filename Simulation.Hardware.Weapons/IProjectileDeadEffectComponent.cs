using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileDeadEffectComponent
	{
		GameObject prefab
		{
			get;
		}

		GameObject prefab_E
		{
			get;
		}
	}
}
