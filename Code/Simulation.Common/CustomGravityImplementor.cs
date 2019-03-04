using UnityEngine;

namespace Simulation.Common
{
	internal class CustomGravityImplementor : MonoBehaviour, IGravityComponent
	{
		[SerializeField]
		private Vector3 _gravity;

		public Vector3 gravity => _gravity;

		public CustomGravityImplementor()
			: this()
		{
		}
	}
}
