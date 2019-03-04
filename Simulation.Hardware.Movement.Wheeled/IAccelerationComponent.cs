namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IAccelerationComponent
	{
		float avgAcceleration
		{
			get;
			set;
		}

		float avgSteeringForceMultiplier
		{
			get;
			set;
		}

		float avgTimeToMaxAcceleration
		{
			get;
			set;
		}

		float accelerationScaler
		{
			get;
			set;
		}
	}
}
