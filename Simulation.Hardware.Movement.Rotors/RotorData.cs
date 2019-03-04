using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class RotorData
	{
		public bool initialised;

		public bool hoveringRotor;

		public CubeFace facingDirection;

		public bool xInputFlipped;

		public Vector3 prevLocalPos = Vector3.get_zero();

		public Vector3 localVel = Vector3.get_zero();

		public Vector3 prevWorldPos = Vector3.get_zero();

		public Vector3 worldVel = Vector3.get_zero();
	}
}
