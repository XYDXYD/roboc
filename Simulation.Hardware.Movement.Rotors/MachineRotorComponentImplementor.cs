using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal class MachineRotorComponentImplementor : IAudioLiftingLoweringComponent, IRotorPowerValueComponent, IPlayingAudioComponent, IMachineRotorAudioLevelComponent, IRotorInputComponent, IAverageMovementValuesComponent, ITimerComponent, ILocalCOMComponent, IRotorsGroundedComponent, ICacheUpdateComponent, IMachineTiltComponent, IMachineDriftComponent, IRobotHeightComponent, IMassComponent, IForceOffsetComponent, IMovementStoppingParamsComponent
	{
		public float stoppingDampingScale_ = 0.25f;

		public float slowingDampingScale_ = 0.25f;

		private string _playingAudio;

		private float _powerChangeRate = 1f;

		public bool lifting
		{
			get;
			set;
		}

		public bool lowering
		{
			get;
			set;
		}

		public float currentPower
		{
			get;
			set;
		}

		public float power
		{
			get;
			set;
		}

		public float powerChangeRate => _powerChangeRate;

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

		public string playingAudio
		{
			get
			{
				return _playingAudio;
			}
			set
			{
				_playingAudio = value;
			}
		}

		public bool isAudioPlaying
		{
			get;
			set;
		}

		public float prevLoadParam
		{
			get;
			set;
		}

		public float prevLowerParam
		{
			get;
			set;
		}

		public float prevRiseParam
		{
			get;
			set;
		}

		public int audioLevel
		{
			get;
			set;
		}

		public bool inputRise
		{
			get;
			set;
		}

		public bool inputLower
		{
			get;
			set;
		}

		public bool inputRight
		{
			get;
			set;
		}

		public bool inputLeft
		{
			get;
			set;
		}

		public bool inputForward
		{
			get;
			set;
		}

		public bool inputBack
		{
			get;
			set;
		}

		public bool inputLegacyStrafeLeft
		{
			get;
			set;
		}

		public bool inputLegacyStrafeRight
		{
			get;
			set;
		}

		public float avgCeilingHeightModifier
		{
			get;
			set;
		}

		public float maxCarryingMass
		{
			get;
			set;
		}

		public float avgMaxHeightChangeSpeed
		{
			get;
			set;
		}

		public float avgHeightAcceleration
		{
			get;
			set;
		}

		public float avgStrafeAcceleration
		{
			get;
			set;
		}

		public float avgTurnAcceleration
		{
			get;
			set;
		}

		public float avgTurnMaxRate
		{
			get;
			set;
		}

		public float avgTurnTangentalAcceleration
		{
			get;
			set;
		}

		public float avgTurnTangentalMaxSpeed
		{
			get;
			set;
		}

		public float avgLevelAcceleration
		{
			get;
			set;
		}

		public float avgLevelRate
		{
			get;
			set;
		}

		public float avgDriftAcceleration
		{
			get;
			set;
		}

		public float avgDriveMaxSpeed
		{
			get;
			set;
		}

		public float avgDriftMaxSpeedAngle
		{
			get;
			set;
		}

		public float avgRotorSize
		{
			get;
			set;
		}

		public float avgZeroTiltSize
		{
			get;
			set;
		}

		public float avgTilt
		{
			get;
			set;
		}

		public float avgMovementTilt
		{
			get;
			set;
		}

		public float avgBankTilt
		{
			get;
			set;
		}

		public float avgFullHoverAngle
		{
			get;
			set;
		}

		public float avgMinHoverAngle
		{
			get;
			set;
		}

		public float avgMinHoverRatio
		{
			get;
			set;
		}

		public float avgHoverRadiusSqr
		{
			get;
			set;
		}

		public Vector3 avgForcePos
		{
			get;
			set;
		}

		public float timer
		{
			get;
			set;
		}

		public Vector3 localCOM
		{
			get;
			set;
		}

		public bool grounded
		{
			get;
			set;
		}

		public bool prevDescending
		{
			get;
			set;
		}

		public Vector3 prevPosition
		{
			get;
			set;
		}

		public bool updateRequired
		{
			get;
			set;
		}

		public Vector3 localMovementTilt
		{
			get;
			set;
		}

		public Vector3 targetLocalTilt
		{
			get;
			set;
		}

		public Vector3 localBalanceTilt
		{
			get;
			set;
		}

		public Vector3 targetLocalDrift
		{
			get;
			set;
		}

		public float targetDriftSpeed
		{
			get;
			set;
		}

		public float robotHeight
		{
			get;
			set;
		}

		public float mass
		{
			get;
			set;
		}

		public float modifiedMass
		{
			get;
			set;
		}

		public Vector3 localForceOffset
		{
			get;
			set;
		}

		public float stoppingDampingScale => stoppingDampingScale_;

		public float slowingDampingScale => slowingDampingScale_;

		public MachineRotorComponentImplementor(bool isLocalPlayer)
		{
			_playingAudio = ((!isLocalPlayer) ? "RotorBlades_Timeline_Enemy" : "RotorBlades_Timeline");
		}
	}
}
