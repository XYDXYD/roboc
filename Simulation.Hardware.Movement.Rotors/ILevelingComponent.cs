namespace Simulation.Hardware.Movement.Rotors
{
	internal interface ILevelingComponent
	{
		float levelAcceleration
		{
			get;
		}

		float levelMaxRate
		{
			get;
		}
	}
}
