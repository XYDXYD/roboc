using UnityEngine;

namespace Simulation.Hardware
{
	internal class MachineRigidbodyTransformImplementor : IRigidBodyComponent, ITransformComponent, IThreadSafeRigidBodyComponent, IThreadSafeTransformComponent
	{
		private Rigidbody _rigidbody;

		private Transform _transform;

		public Rigidbody rb => _rigidbody;

		public Transform T => _transform;

		public RigidbodyThreadSafe RBTS
		{
			get;
			set;
		}

		public TransformThreadSafe TTS
		{
			get;
			set;
		}

		public MachineRigidbodyTransformImplementor(Rigidbody rb)
		{
			_rigidbody = rb;
			_transform = _rigidbody.get_transform();
		}
	}
}
