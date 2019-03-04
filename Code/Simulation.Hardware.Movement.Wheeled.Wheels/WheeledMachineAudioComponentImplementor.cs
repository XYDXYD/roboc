namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal class WheeledMachineAudioComponentImplementor : IMotorAudioStateComponent, IWheelAudioStateComponent, IAudioNameComponent
	{
		private string _wheelSound = string.Empty;

		private string _motorSound = string.Empty;

		public string wheelSoundEvent => _wheelSound;

		public string motorSoundEvent => _motorSound;

		public float wheelspinScale
		{
			get;
			set;
		}

		public float sidewaysSkidScale
		{
			get;
			set;
		}

		public bool wheelSoundPlaying
		{
			get;
			set;
		}

		public bool motorSoundPlaying
		{
			get;
			set;
		}

		public float avgLevel
		{
			get;
			set;
		}

		public float totalRPMScale
		{
			get;
			set;
		}

		public float totalLoadScale
		{
			get;
			set;
		}

		public float cameraDistance
		{
			get;
			set;
		}

		public WheeledMachineAudioComponentImplementor(bool isLocalPlayer)
		{
			_wheelSound = "KUB_DEMO_fabric_Wheels";
			_motorSound = ((!isLocalPlayer) ? "Motor_Timeline_Enemy" : "Motor_Timeline");
		}
	}
}
