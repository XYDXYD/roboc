namespace Simulation.Hardware.Movement.Thruster
{
	internal interface IThrusterAudioStateComponent
	{
		bool isJetPlaying
		{
			get;
			set;
		}

		float playStopTimePerLevel
		{
			get;
			set;
		}

		float lastRamp
		{
			get;
			set;
		}

		int level
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
