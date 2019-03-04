namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal interface IWheelSuspensionComponent
	{
		float fullSuspensionDistance
		{
			get;
			set;
		}

		float wheelForwardOffset
		{
			get;
			set;
		}
	}
}
