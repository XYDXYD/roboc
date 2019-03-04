using UnityEngine;

namespace Simulation
{
	internal interface IPowerModuleEffectsComponent
	{
		GameObject LocalPlayerEffectPrefab
		{
			get;
		}

		GameObject AllyEffectPrefab
		{
			get;
		}

		GameObject EnemyEffectPrefab
		{
			get;
		}
	}
}
