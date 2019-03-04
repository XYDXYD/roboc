namespace Simulation.Hardware.Movement.Rotors
{
	internal interface ITiltComponent
	{
		float rotorRadius
		{
			get;
		}

		float zeroTiltRadius
		{
			get;
		}

		float tiltDegrees
		{
			get;
		}

		float movementTilt
		{
			get;
		}

		float bankTilt
		{
			get;
		}

		float fullHoverAngle
		{
			get;
		}

		float minHoverAngle
		{
			get;
		}

		float minHoverRatio
		{
			get;
		}
	}
}
