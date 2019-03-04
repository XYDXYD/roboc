using UnityEngine;

namespace Simulation.Hardware.Movement.Thruster
{
	internal class ThrusterComponentImplementor : MonoBehaviour, IFacingDirectionComponent, IThrusterForceComponent, IMaxSpeedComponent, IInputReceivedComponent, IThrusterParticleEffectsComponent, IMovementStoppingParamsComponent, IPartLevelComponent, IVerticalOrientationThrusterCountComponent, IThrusterForceAppliedComponent, IAccelerationDelayComponent, ITypeComponent, ISpeedModifierComponent, IMaxCarryMassComponent, IValidMovementComponent
	{
		public int level = 1;

		public float force = 700f;

		public float accelerationDelay;

		public ParticleSystem[] jetParticleSystems;

		public float _stoppingDampingScale = 0.1f;

		public float _slowingDampingScale = 0.1f;

		public bool verticalStrafingEnabled;

		public ThrusterType _type;

		private bool _alignmentRectifierActive;

		private CubeFace _legacyFacingDirection;

		private CubeFace _actualFacingDirection;

		private CubeFace _pitchFacingDirection;

		private float _accelerationPercent = 1f;

		private Vector3 _positiveAxisMaxSpeed = Vector3.get_zero();

		private Vector3 _negativeAxisMaxSpeed = Vector3.get_zero();

		private Transform _transform;

		int IPartLevelComponent.level
		{
			get
			{
				return level;
			}
		}

		CubeFace IFacingDirectionComponent.legacyDirection
		{
			get
			{
				return _legacyFacingDirection;
			}
		}

		CubeFace IFacingDirectionComponent.actualDirection
		{
			get
			{
				return _actualFacingDirection;
			}
		}

		CubeFace IFacingDirectionComponent.pitchDirection
		{
			get
			{
				return _pitchFacingDirection;
			}
		}

		float IThrusterForceComponent.force
		{
			get
			{
				return force;
			}
		}

		ParticleSystem[] IThrusterParticleEffectsComponent.particleSystems
		{
			get
			{
				return jetParticleSystems;
			}
		}

		float IMovementStoppingParamsComponent.stoppingDampingScale
		{
			get
			{
				return _stoppingDampingScale;
			}
		}

		float IMovementStoppingParamsComponent.slowingDampingScale
		{
			get
			{
				return _slowingDampingScale;
			}
		}

		int IVerticalOrientationThrusterCountComponent.numUpThrusters
		{
			get;
			set;
		}

		int IVerticalOrientationThrusterCountComponent.numDownThrusters
		{
			get;
			set;
		}

		float IAccelerationDelayComponent.accelerationDelay
		{
			get
			{
				return accelerationDelay;
			}
		}

		public Vector3 direction => (type != 0) ? _transform.get_up() : _transform.get_forward();

		public float received
		{
			get;
			set;
		}

		public bool forceApplied
		{
			get;
			set;
		}

		public Vector3 forceDirection
		{
			get;
			set;
		}

		public ThrusterType type => _type;

		public float accelerationPercent
		{
			get
			{
				return _accelerationPercent;
			}
			set
			{
				_accelerationPercent = value;
			}
		}

		public float startApplyForceTime
		{
			get;
			set;
		}

		public Vector3 positiveAxisMaxSpeed
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _positiveAxisMaxSpeed;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_positiveAxisMaxSpeed = value;
			}
		}

		public Vector3 negativeAxisMaxSpeed
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _negativeAxisMaxSpeed;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_negativeAxisMaxSpeed = value;
			}
		}

		public float maxSpeed
		{
			get;
			set;
		}

		public float speedModifier => 1f;

		public bool isValid => true;

		public bool affectsMaxSpeed => false;

		public float maxCarryMass
		{
			get;
			set;
		}

		public float minItemsModifier
		{
			get;
			set;
		}

		public ThrusterComponentImplementor()
			: this()
		{
		}//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)


		private void Awake()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			_transform = this.get_transform();
			_positiveAxisMaxSpeed = Vector3.get_forward();
			_negativeAxisMaxSpeed = ((type != 0) ? Vector3.get_forward() : Vector3.get_zero());
			minItemsModifier = 1f;
		}

		public void SetFacingDirection(CubeFace facingDirection)
		{
			_legacyFacingDirection = facingDirection;
			_actualFacingDirection = facingDirection;
			_pitchFacingDirection = facingDirection;
			ComputeMaxSpeed(_actualFacingDirection);
		}

		private void ComputeMaxSpeed(CubeFace facingDirection)
		{
			switch (facingDirection)
			{
			case CubeFace.Front:
				break;
			case CubeFace.Other:
				break;
			case CubeFace.Up:
				Swap(ref _positiveAxisMaxSpeed.z, ref _positiveAxisMaxSpeed.y);
				Swap(ref _negativeAxisMaxSpeed.z, ref _negativeAxisMaxSpeed.y);
				break;
			case CubeFace.Down:
				Swap(ref _positiveAxisMaxSpeed.z, ref _negativeAxisMaxSpeed.y);
				Swap(ref _negativeAxisMaxSpeed.z, ref _positiveAxisMaxSpeed.y);
				break;
			case CubeFace.Back:
				Swap(ref _negativeAxisMaxSpeed.z, ref _positiveAxisMaxSpeed.z);
				break;
			case CubeFace.Right:
				Swap(ref _positiveAxisMaxSpeed.z, ref _positiveAxisMaxSpeed.x);
				Swap(ref _negativeAxisMaxSpeed.z, ref _negativeAxisMaxSpeed.x);
				break;
			case CubeFace.Left:
				Swap(ref _positiveAxisMaxSpeed.z, ref _negativeAxisMaxSpeed.x);
				Swap(ref _negativeAxisMaxSpeed.z, ref _positiveAxisMaxSpeed.x);
				break;
			}
		}

		private void Swap(ref float a, ref float b)
		{
			float num = a;
			a = b;
			b = num;
		}

		public void SetFacingDirection(CubeFace legacyFacingDirection, CubeFace actualFacingDirection, CubeFace pitchFacingDirection)
		{
			_legacyFacingDirection = legacyFacingDirection;
			_actualFacingDirection = actualFacingDirection;
			_pitchFacingDirection = pitchFacingDirection;
			ComputeMaxSpeed(_actualFacingDirection);
		}
	}
}
