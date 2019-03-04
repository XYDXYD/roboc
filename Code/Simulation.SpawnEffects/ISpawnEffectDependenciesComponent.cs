using Svelto.DataStructures;
using UnityEngine;

namespace Simulation.SpawnEffects
{
	internal interface ISpawnEffectDependenciesComponent
	{
		Rigidbody rigidbody
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

		FasterList<Renderer> allRenderers
		{
			get;
		}

		bool robotAnimating
		{
			get;
			set;
		}
	}
}
