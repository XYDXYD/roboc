namespace Simulation.Hardware.Movement.Thruster
{
	internal interface IVerticalOrientationThrusterCountComponent
	{
		int numUpThrusters
		{
			get;
			set;
		}

		int numDownThrusters
		{
			get;
			set;
		}
	}
}
