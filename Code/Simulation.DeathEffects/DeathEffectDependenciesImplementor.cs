using UnityEngine;

namespace Simulation.DeathEffects
{
	internal class DeathEffectDependenciesImplementor : IDeathEffectDependenciesComponent
	{
		public Rigidbody rigidbody
		{
			get;
			private set;
		}

		public GameObject root
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

		public DeathEffectDependenciesImplementor(Rigidbody rigidbody_, GameObject root_, Vector3 machineCenter_, Vector3 machineSize_)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			rigidbody = rigidbody_;
			root = root_;
			machineCenter = machineCenter_;
			machineSize = machineSize_;
		}
	}
}
