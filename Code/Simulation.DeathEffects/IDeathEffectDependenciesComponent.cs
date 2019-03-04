using UnityEngine;

namespace Simulation.DeathEffects
{
	internal interface IDeathEffectDependenciesComponent
	{
		Rigidbody rigidbody
		{
			get;
		}

		GameObject root
		{
			get;
		}

		Vector3 machineCenter
		{
			get;
		}

		Vector3 machineSize
		{
			get;
		}
	}
}
