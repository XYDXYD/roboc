namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface ITrackTurningToDriveDirectionComponent
	{
		bool turning
		{
			get;
			set;
		}

		bool previousTurningState
		{
			get;
			set;
		}
	}
}
