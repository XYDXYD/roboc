using Svelto.DataStructures;
using UnityEngine;

namespace Simulation.SpawnEffects
{
	internal class SpawnEffectDependenciesImplementor : ISpawnEffectDependenciesComponent
	{
		public Rigidbody rigidbody
		{
			get;
			private set;
		}

		public Vector3 machineCenter
		{
			get;
			private set;
		}

		public Vector3 machineSize
		{
			get;
			private set;
		}

		public FasterList<Renderer> allRenderers
		{
			get;
			private set;
		}

		public bool robotAnimating
		{
			get;
			set;
		}

		public SpawnEffectDependenciesImplementor(Rigidbody rigidbody_, Vector3 machineCenter_, Vector3 machineSize_, FasterList<Renderer> allRenderers_)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			rigidbody = rigidbody_;
			machineCenter = machineCenter_;
			machineSize = machineSize_;
			allRenderers = allRenderers_;
		}
	}
}
