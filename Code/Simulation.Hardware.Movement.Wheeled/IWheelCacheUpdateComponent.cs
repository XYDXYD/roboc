namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IWheelCacheUpdateComponent
	{
		bool updateRequired
		{
			get;
			set;
		}

		bool updateSteeringRequired
		{
			get;
			set;
		}
	}
}
