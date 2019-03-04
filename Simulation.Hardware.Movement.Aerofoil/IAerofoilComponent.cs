using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	public interface IAerofoilComponent
	{
		Transform forceT
		{
			get;
		}

		float elevationSpeed
		{
			get;
		}

		float elevationSpeedHeavy
		{
			get;
		}

		float barrelSpeed
		{
			get;
		}

		float barrelSpeedHeavy
		{
			get;
		}

		float rudderSpeed
		{
			get;
		}

		float rudderSpeedHeavy
		{
			get;
		}

		float thrust
		{
			get;
		}

		float thrustHeavy
		{
			get;
		}

		float vtolVelocity
		{
			get;
		}

		float vtolVelocityHeavy
		{
			get;
		}

		float bankSpeed
		{
			get;
		}

		float bankSpeedHeavy
		{
			get;
		}
	}
}
