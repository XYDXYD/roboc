namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IWheelColliderDataComponent
	{
		WheelColliderData wheelColliderData
		{
			get;
		}

		WheelColliderDataThreadSafe wheelColliderDataThreadSafe
		{
			get;
			set;
		}

		float travel
		{
			get;
			set;
		}
	}
}
