using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal class AerofoilGFXImplementor : MonoBehaviour, IAerofoilGFXComponent
	{
		public Transform FlapT;

		public GameObject OnSFXGameObject;

		public GameObject ThrustSFXGameObject;

		public Transform flapT => FlapT;

		public GameObject onSFXGameObject => OnSFXGameObject;

		public GameObject thrustSFXGameObject => ThrustSFXGameObject;

		public Vector3 lastPos
		{
			get;
			set;
		}

		public AerofoilGFXImplementor()
			: this()
		{
		}
	}
}
