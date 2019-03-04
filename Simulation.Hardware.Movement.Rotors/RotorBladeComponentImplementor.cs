using UnityEngine;

namespace Simulation.Hardware.Movement.Rotors
{
	internal class RotorBladeComponentImplementor : MonoBehaviour, IHeightChangeComponent, IStrafeComponent, ITurningComponent, IDriftComponent, ILevelingComponent, ITiltComponent, IForcePointScaleComponent, ICeilingHeightModifierComponent, IRotorForceMagnitudeComponent, IMaxCarryMassComponent, IRotorDataComponent, IRotorGraphicsComponent, ITiltPivotTransformComponent, ISpinningPivotTransformComponent, IRotorAudioLevelComponent, IMaxSpeedComponent, ISpeedModifierComponent, IValidMovementComponent
	{
		public Transform tiltPivot_;

		public Transform spinningPivot_;

		public float rotorRadius_ = 1f;

		public float zeroTiltRadius_ = 0.2f;

		public float ceilingHeightModifier_ = 1f;

		public float heightAcceleration_ = 1f;

		public float heightMaxChangeSpeed_ = 1f;

		public float strafeAcceleration_ = 1f;

		public float turnAcceleration_ = 90f;

		public float turnMaxRate_ = 90f;

		public float turnTangentalAcceleration_ = 1f;

		public float turnTangentalMaxSpeed_ = 10f;

		public float levelAcceleration_ = 90f;

		public float levelMaxRate_ = 90f;

		public float driftAcceleration_ = 1f;

		public float driftMaxSpeedAngle_ = 30f;

		public float tiltDegrees_ = 15f;

		public float movementTilt_ = 15f;

		public float bankTilt_ = 30f;

		public float forcePointScale_ = 1f;

		public float fullHoverAngle_ = 45f;

		public float minHoverAngle_ = 90f;

		public float minHoverRatio_ = 0.5f;

		public float forceMagnitude_ = 1f;

		public int audioLevel_;

		private RotorData _rotorData;

		public RotorGraphics graphics;

		public float heightMaxChangeSpeed => heightMaxChangeSpeed_;

		public float heightAcceleration => heightAcceleration_;

		public float strafeAcceleration => strafeAcceleration_;

		public float turnAcceleration => turnAcceleration_;

		public float turnMaxRate => turnMaxRate_;

		public float turnTangentalAcceleration => turnTangentalAcceleration_;

		public float turnTangentalMaxSpeed => turnTangentalMaxSpeed_;

		public float driftAcceleration => driftAcceleration_;

		public float driftMaxSpeedAngle => driftMaxSpeedAngle_;

		public float levelAcceleration => levelAcceleration_;

		public float levelMaxRate => levelMaxRate_;

		public float rotorRadius => rotorRadius_;

		public float zeroTiltRadius => zeroTiltRadius_;

		public float tiltDegrees => tiltDegrees_;

		public float movementTilt => movementTilt_;

		public float bankTilt => bankTilt_;

		public float fullHoverAngle => fullHoverAngle_;

		public float minHoverAngle => minHoverAngle_;

		public float minHoverRatio => minHoverRatio_;

		public float forcePointScale => forcePointScale_;

		public float ceilingHeightModifier => ceilingHeightModifier_;

		public float forceMagnitude => forceMagnitude_;

		public float maxCarryMass
		{
			get;
			set;
		}

		public RotorData rotorData => _rotorData;

		public RotorGraphics rotorGraphics => graphics;

		public Transform tiltPivot => tiltPivot_;

		public Transform spinningPivot => spinningPivot_;

		public int audioLevel => audioLevel_;

		public float maxSpeed
		{
			get;
			set;
		}

		public Vector3 positiveAxisMaxSpeed
		{
			get;
			set;
		}

		public Vector3 negativeAxisMaxSpeed
		{
			get;
			set;
		}

		public float speedModifier => 1f;

		public bool isValid => true;

		public bool affectsMaxSpeed => true;

		public float minItemsModifier
		{
			get;
			set;
		}

		public RotorBladeComponentImplementor()
			: this()
		{
		}

		private void Awake()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			_rotorData = new RotorData();
			positiveAxisMaxSpeed = Vector3.get_one();
			negativeAxisMaxSpeed = Vector3.get_one();
			minItemsModifier = 1f;
			graphics.InitialiseRotorGraphics(this.get_transform());
		}
	}
}
