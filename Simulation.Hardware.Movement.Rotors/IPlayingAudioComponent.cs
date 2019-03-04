namespace Simulation.Hardware.Movement.Rotors
{
	internal interface IPlayingAudioComponent
	{
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

		bool isAudioPlaying
		{
			get;
			set;
		}

		string playingAudio
		{
			get;
			set;
		}

		float prevLoadParam
		{
			get;
			set;
		}

		float prevLowerParam
		{
			get;
			set;
		}

		float prevRiseParam
		{
			get;
			set;
		}
	}
}
