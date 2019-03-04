namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IAudioLiftingLoweringComponent
	{
		bool lifting
		{
			get;
			set;
		}

		bool lowering
		{
			get;
			set;
		}
	}
}
