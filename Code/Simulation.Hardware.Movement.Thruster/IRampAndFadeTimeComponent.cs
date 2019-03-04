namespace Simulation.Hardware.Movement.Thruster
{
	internal interface IRampAndFadeTimeComponent
	{
		float totalRampUpTime
		{
			get;
		}

		float totalFadeTime
		{
			get;
		}
	}
}
