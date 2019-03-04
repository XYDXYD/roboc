namespace Simulation.Hardware.Weapons.Nano
{
	internal interface INanoBeamAudioComponent
	{
		string laserStartAudio
		{
			get;
		}

		string laserLoopAudio
		{
			get;
		}

		string laserEndAudio
		{
			get;
		}

		string disruptorStartAudio
		{
			get;
		}

		string disruptorLoopAudio
		{
			get;
		}

		string disruptorEndAudio
		{
			get;
		}

		string hitGroundAudio
		{
			get;
		}
	}
}
