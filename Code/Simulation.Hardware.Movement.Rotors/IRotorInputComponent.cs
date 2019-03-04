namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IRotorInputComponent
	{
		bool inputRise
		{
			get;
			set;
		}

		bool inputLower
		{
			get;
			set;
		}

		bool inputRight
		{
			get;
			set;
		}

		bool inputLeft
		{
			get;
			set;
		}

		bool inputForward
		{
			get;
			set;
		}

		bool inputBack
		{
			get;
			set;
		}

		bool inputLegacyStrafeLeft
		{
			get;
			set;
		}

		bool inputLegacyStrafeRight
		{
			get;
			set;
		}
	}
}
