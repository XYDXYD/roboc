namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface ILateralAccelerationComponent
	{
		bool motorized
		{
			get;
		}

		float acceleration
		{
			get;
		}

		float timeToMaxAcceleration
		{
			get;
		}
	}
}
