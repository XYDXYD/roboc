namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface IMaxSpeedRatioComponent
	{
		float maxSpeedRatio
		{
			get;
			set;
		}

		float prevMaxSpeedRatio
		{
			get;
			set;
		}
	}
}
