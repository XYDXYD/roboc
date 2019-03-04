using UnityEngine;

namespace Simulation
{
	internal struct NodeBoxCollider
	{
		public GameObject gameobject;

		public IClusteredColliderNode node;

		public BoxCollider collider;

		public NodeBoxCollider(IClusteredColliderNode node, BoxCollider collider)
		{
			gameobject = collider.get_gameObject();
			this.node = node;
			this.collider = collider;
		}
	}
}
