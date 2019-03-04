using UnityEngine;

namespace Simulation
{
	internal interface IRadarVFXComponent
	{
		GameObject activationVfxPrefab
		{
			get;
		}

		GameObject enemyActivationVfxPrefab
		{
			get;
		}

		Transform vfxAnchor
		{
			get;
		}

		Animator animatorComponent
		{
			get;
		}
	}
}
