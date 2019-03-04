namespace Simulation.Hardware.Movement.Thruster
{
	public class ThrusterAudioComponentImplementor : IAudioUpdateTimeComponent, IThrusterAudioNamesComponent, IThrusterAudioStateComponent, IRampAndFadeTimeComponent
	{
		private float _totalRampUpTime = 1f;

		private float _totalFadeTime = 1f;

		private string _loopSound;

		private float _timeToUpdate;

		public float timeToUpdate
		{
			get
			{
				return _timeToUpdate;
			}
			set
			{
				_timeToUpdate = value;
			}
		}

		public string loopSound => _loopSound;

		public int level
		{
			get;
			set;
		}

		public bool isJetPlaying
		{
			get;
			set;
		}

		public float playStopTimePerLevel
		{
			get;
			set;
		}

		public float lastRamp
		{
			get;
			set;
		}

		public float cameraDistance
		{
			get;
			set;
		}

		public float totalRampUpTime => _totalRampUpTime;

		public float totalFadeTime => _totalFadeTime;

		public ThrusterAudioComponentImplementor(bool isLocalPlayer)
		{
			_loopSound = ((!isLocalPlayer) ? "Jet_Timeline_Enemy" : "Jet_Timeline");
		}
	}
}
