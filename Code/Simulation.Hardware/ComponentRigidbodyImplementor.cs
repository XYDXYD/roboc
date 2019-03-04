using UnityEngine;

namespace Simulation.Hardware
{
	internal class ComponentRigidbodyImplementor : IRigidBodyComponent, IThreadSafeRigidBodyComponent, IImplementor
	{
		private Rigidbody _rigidbody;

		public Rigidbody rb => _rigidbody;

		public RigidbodyThreadSafe RBTS
		{
			get;
			set;
		}

		public ComponentRigidbodyImplementor(Rigidbody rb)
		{
			_rigidbody = rb;
		}
	}
}
