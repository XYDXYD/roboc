namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface ISpeedComponent
	{
		float currentSpeed
		{
			get;
			set;
		}

		bool movingBackwards
		{
			get;
			set;
		}
	}
}
