namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IDriftComponent
	{
		float driftAcceleration
		{
			get;
		}

		float driftMaxSpeedAngle
		{
			get;
		}
	}
}
