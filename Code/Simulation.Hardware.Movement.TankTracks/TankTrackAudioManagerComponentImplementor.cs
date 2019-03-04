namespace Simulation.Hardware.Movement.TankTracks
{
	public class TankTrackAudioManagerComponentImplementor : ITankTrackAudioEventComponent, ITankTrackAudioPlayingComponent, IMaxSpeedRatioComponent
	{
		public string audioEvent
		{
			get;
			set;
		}

		public bool audioPlaying
		{
			get;
			set;
		}

		public float avgLevel
		{
			get;
			set;
		}

		public float cameraDistance
		{
			get;
			set;
		}

		public float maxSpeedRatio
		{
			get;
			set;
		}

		public float prevMaxSpeedRatio
		{
			get;
			set;
		}

		public TankTrackAudioManagerComponentImplementor(bool isLocalPlayer)
		{
			audioEvent = ((!isLocalPlayer) ? "TankTracks_Timeline_Enemy" : "TankTracks_Timeline");
		}
	}
}
