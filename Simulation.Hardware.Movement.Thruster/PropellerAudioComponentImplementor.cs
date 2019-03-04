using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	public class PropellerAudioComponentImplementor : IPropellerAudioNamesComponent, IPropellerAudioStateComponent
	{
		private string _soundEvent;

		public string soundEvent => _soundEvent;

		public bool isPlaying
		{
			get;
			set;
		}

		public float lastSpinParam
		{
			get;
			set;
		}

		public float lastTurnParam
		{
			get;
			set;
		}

		public float lastLiftParam
		{
			get;
			set;
		}

		public Vector3 previousForward
		{
			get;
			set;
		}

		public int maxLevelPlaying
		{
			get;
			set;
		}

		public float cameraDistance
		{
			get;
			set;
		}

		public PropellerAudioComponentImplementor(bool isLocalPlayer)
		{
			_soundEvent = ((!isLocalPlayer) ? "Propeller_Timeline_Enemy" : "Propeller_Timeline");
		}
	}
}
