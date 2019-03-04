namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface INumGroundedWheelsComponent
	{
		int groundedParts
		{
			get;
			set;
		}

		int groundedMotorizedParts
		{
			get;
			set;
		}
	}
}
