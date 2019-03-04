namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface ITrackStoppedComponent
	{
		bool stopped
		{
			get;
			set;
		}

		bool previousStoppedState
		{
			get;
			set;
		}
	}
}
