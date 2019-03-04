namespace Simulation.Hardware.Movement.TankTracks
{
	internal interface ITankTrackAudioPlayingComponent
	{
		bool audioPlaying
		{
			get;
			set;
		}

		float avgLevel
		{
			get;
			set;
		}

		float cameraDistance
		{
			get;
			set;
		}
	}
}
