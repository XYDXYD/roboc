namespace Simulation.Hardware.Movement.Rotors
{
	internal interface ITurningComponent
	{
		float turnAcceleration
		{
			get;
		}

		float turnMaxRate
		{
			get;
		}

		float turnTangentalAcceleration
		{
			get;
		}

		float turnTangentalMaxSpeed
		{
			get;
		}
	}
}
