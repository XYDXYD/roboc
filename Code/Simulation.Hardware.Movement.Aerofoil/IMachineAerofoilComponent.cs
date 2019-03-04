using UnityEngine;

namespace Simulation.Hardware.Movement.Aerofoil
{
	public interface IMachineAerofoilComponent
	{
		float rudderSpeed
		{
			get;
			set;
		}

		float rudderSpeedHeavy
		{
			get;
			set;
		}

		Vector3 lateralForceOffset
		{
			get;
			set;
		}

		float bankSpeed
		{
			get;
			set;
		}

		float bankSpeedHeavy
		{
			get;
			set;
		}

		float barrelSpeed
		{
			get;
			set;
		}

		float barrelSpeedHeavy
		{
			get;
			set;
		}

		float elevationSpeed
		{
			get;
			set;
		}

		float elevationSpeedHeavy
		{
			get;
			set;
		}

		float stopperCapacityRatio
		{
			get;
			set;
		}

		float thrust
		{
			get;
			set;
		}

		float thrustHeavy
		{
			get;
			set;
		}

		float vtolVelocity
		{
			get;
			set;
		}

		float vtolVelocityHeavy
		{
			get;
			set;
		}

		float liftCapacityRatio
		{
			get;
			set;
		}

		float lateralCapacityRatio
		{
			get;
			set;
		}

		float barrelCapacityRatio
		{
			get;
			set;
		}

		float bankCapacityRatio
		{
			get;
			set;
		}

		float rudderCapacityRatio
		{
			get;
			set;
		}

		float speedRatio
		{
			get;
			set;
		}

		float barrelOffset
		{
			get;
			set;
		}

		float elevationOffset
		{
			get;
			set;
		}

		float[] aDragParams
		{
			get;
			set;
		}

		bool updateRequired
		{
			get;
			set;
		}

		int numAerofoils
		{
			get;
			set;
		}

		Vector3 aerofoilInput
		{
			get;
			set;
		}
	}
}
