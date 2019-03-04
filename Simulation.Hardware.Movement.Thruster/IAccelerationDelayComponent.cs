namespace Simulation.Hardware.Movement.Thruster
{
	internal interface IAccelerationDelayComponent
	{
		float accelerationDelay
		{
			get;
		}

		float accelerationPercent
		{
			get;
			set;
		}

		float startApplyForceTime
		{
			get;
			set;
		}
	}
}
