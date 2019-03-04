namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IAngularVelocityComponent
	{
		float prevRightAngularSpeed
		{
			get;
			set;
		}

		float avgAngularDamping
		{
			get;
			set;
		}
	}
}
