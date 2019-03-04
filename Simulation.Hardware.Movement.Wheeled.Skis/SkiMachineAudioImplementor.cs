namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal sealed class SkiMachineAudioImplementor : ISkiAudioStateComponent
	{
		public bool loopSoundPlaying
		{
			get;
			set;
		}

		public float speedScale
		{
			get;
			set;
		}

		public bool turnSoundPlaying
		{
			get;
			set;
		}
	}
}
