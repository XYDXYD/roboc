using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal interface IProjectileEffectImpactSuccessfulComponent
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

		string audioEventHitMe
		{
			get;
		}

		string audioEventEnemyHitOther
		{
			get;
		}
	}
}
