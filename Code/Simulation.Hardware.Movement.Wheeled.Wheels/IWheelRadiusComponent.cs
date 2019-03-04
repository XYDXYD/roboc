namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal interface IWheelRadiusComponent
	{
		float wheelRadius
		{
			get;
			set;
		}

		float inverseCircumference
		{
			get;
			set;
		}
	}
}
