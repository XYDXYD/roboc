namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal interface ISkiAudioStateComponent
	{
		bool loopSoundPlaying
		{
			get;
			set;
		}

		bool turnSoundPlaying
		{
			get;
			set;
		}

		float speedScale
		{
			get;
			set;
		}
	}
}
