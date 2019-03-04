using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled
{
	internal struct WheelColliderDataThreadSafe
	{
		public float radius;

		public float suspensionDistance;

		public Vector3 inversePos;

		public float steerAngle;

		public Vector3 position;
	}
}
