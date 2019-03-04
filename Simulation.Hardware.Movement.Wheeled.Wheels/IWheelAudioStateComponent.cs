namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal interface IWheelAudioStateComponent
	{
		bool wheelSoundPlaying
		{
			get;
			set;
		}

		float wheelspinScale
		{
			get;
			set;
		}

		float sidewaysSkidScale
		{
			get;
			set;
		}
	}
}
