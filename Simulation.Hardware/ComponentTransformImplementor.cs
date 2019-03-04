using UnityEngine;

namespace Simulation.Hardware
{
	internal class ComponentTransformImplementor : MonoBehaviour, ITransformComponent, IThreadSafeTransformComponent, IImplementor
	{
		public Transform T => this.get_transform();

		public TransformThreadSafe TTS
		{
			get;
			set;
		}

		public ComponentTransformImplementor()
			: this()
		{
		}
	}
}
