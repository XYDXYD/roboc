namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal interface IMotorAudioStateComponent
	{
		bool motorSoundPlaying
		{
			get;
			set;
		}

		float totalLoadScale
		{
			get;
			set;
		}

		float avgLevel
		{
			get;
			set;
		}

		float totalRPMScale
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
