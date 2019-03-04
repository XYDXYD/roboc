using UnityEngine;

namespace Simulation.Hardware
{
	internal struct RigidbodyThreadSafe
	{
		public Vector3 angularVelocity;

		public Vector3 worldCenterOfMass;

		public Vector3 velocity;

		public float mass;

		public RigidbodyThreadSafe(Vector3 worldCenterOfMass, Vector3 angularVelocity, Vector3 velocity, float mass)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			this.worldCenterOfMass = worldCenterOfMass;
			this.angularVelocity = angularVelocity;
			this.velocity = velocity;
			this.mass = mass;
		}
	}
}
