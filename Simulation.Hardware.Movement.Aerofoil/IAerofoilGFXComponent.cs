using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	public interface IAerofoilGFXComponent
	{
		Vector3 lastPos
		{
			get;
			set;
		}

		Transform flapT
		{
			get;
		}

		GameObject onSFXGameObject
		{
			get;
		}

		GameObject thrustSFXGameObject
		{
			get;
		}
	}
}
