namespace Simulation.Hardware.Movement.Wheeled
{
	internal interface IAntirollComponent
	{
		float antirollForce
		{
			get;
		}

		bool antiRollForceApplied
		{
			get;
			set;
		}
	}
}
