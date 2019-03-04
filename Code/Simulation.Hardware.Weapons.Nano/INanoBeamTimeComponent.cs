namespace Simulation.Hardware.Weapons.Nano
{
	internal interface INanoBeamTimeComponent
	{
		float endTime
		{
			get;
			set;
		}

		float beamMaxDuration
		{
			get;
		}
	}
}
