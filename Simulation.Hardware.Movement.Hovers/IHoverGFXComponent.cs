using UnityEngine;

namespace Simulation.Hardware.Movement.Hovers
{
	public interface IHoverGFXComponent
	{
		Vector3 currentLookAtOffset
		{
			get;
			set;
		}

		Transform orientation
		{
			get;
		}

		Vector3 previousThrustPost
		{
			get;
			set;
		}

		float minSpinVel
		{
			get;
		}

		float spinSpeedMult
		{
			get;
		}

		Vector3 spinAxisV
		{
			get;
		}

		Transform spinBlade
		{
			get;
		}

		float currentSpinRotation
		{
			get;
			set;
		}
	}
}
