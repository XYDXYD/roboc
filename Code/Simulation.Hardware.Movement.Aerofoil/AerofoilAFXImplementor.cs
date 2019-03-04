using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class AerofoilAFXImplementor : MonoBehaviour, IAerofoilAFXComponent
	{
		public float AudioLevel;

		public float audioLevel => AudioLevel;

		public AerofoilAFXImplementor()
			: this()
		{
		}
	}
}
