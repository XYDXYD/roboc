using UnityEngine;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal class WheelComponentImplementor : MonoBehaviour, IMachineSideComponent, IGroundedComponent, IPendingForceComponent, IWheelColliderDataComponent, IMaxSpeedComponent, ILateralAccelerationComponent, IFrictionAngleComponent, ICurrentSlopeScalarComponent, IForcePointComponent, IWheelColliderInfo, IPartLevelComponent, IFrictionStiffnessComponent, ISteeringComponent, ICOMDistanceRangeComponent, IBrakeComponent, IAntirollComponent, IWheelLoadComponent, ISlipComponent, IAngularDampingComponent, IDownForceComponent, IGridLocationComponent, IThreadSafeWheelComponent, ISpeedModifierComponent, IMaxCarryMassComponent, IValidMovementComponent
	{
		public bool motorized_ = true;

		public bool steerable_ = true;

		public float maxSteeringReduction_ = 0.7f;

		public float maxSteeringReductionKeyboard_ = 0.7f;

		public float maxSteeringAngle_ = 25f;

		public float maxSteeringAngleKeyboard_ = 25f;

		public float steeringSpeed_ = 100f;

		public float steeringForceMultiplier_ = 1f;

		public int level_ = 1;

		public Transform forcePointTransform_;

		public float lateralAcceleration_ = 5f;

		public float timeToMaxAcceleration_ = 1f;

		public float brakeForce_ = 20f;

		public float angleWithMaxFriction_ = 40f;

		public float angleWithMinFriction_ = 50f;

		public float minFrictionScalar_;

		public float movingFrictionStiffness_ = 2f;

		public float stoppedFrictionStiffness_ = 1f;

		public float antirollForce_ = 10f;

		public float angularDamping_ = 1f;

		public float downAngleForce_ = 15f;

		public WheelFriction regularWheelColliderFriction;

		private WheelColliderData _wheelColliderData;

		private WheelZSide _zSide;

		private WheelXSide _xSide;

		public Byte3 gridPosition
		{
			get;
			set;
		}

		public WheelZSide zSide => _zSide;

		public WheelXSide xSide => _xSide;

		public bool grounded
		{
			get;
			set;
		}

		public float distanceToGround
		{
			get;
			set;
		}

		public Vector3 hitNormal
		{
			get;
			set;
		}

		public Vector3 pendingForce
		{
			get;
			set;
		}

		public Vector3 pendingVelocityChangeForce
		{
			get;
			set;
		}

		public float antirollForce => antirollForce_;

		public WheelColliderData wheelColliderData => _wheelColliderData;

		public WheelColliderDataThreadSafe wheelColliderDataThreadSafe
		{
			get;
			set;
		}

		public Vector3 forcePointComponentTS
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

		public float maxSpeed
		{
			get;
			set;
		}

		public float acceleration => lateralAcceleration_;

		public bool motorized => motorized_;

		public float timeToMaxAcceleration => timeToMaxAcceleration_;

		public float brakeForce => brakeForce_;

		public float angleWithMaxFriction => angleWithMaxFriction_;

		public float angleWithMinFriction => angleWithMinFriction_;

		public float minFrictionScalar => minFrictionScalar_;

		public float currentSlopeScalar
		{
			get;
			set;
		}

		public Transform forcePointTransform => forcePointTransform_;

		public Vector3 forcePoint
		{
			get;
			set;
		}

		public int level => level_;

		public float stoppedFrictionStiffness => stoppedFrictionStiffness_;

		public float movingFrictionStiffness => movingFrictionStiffness_;

		public bool steerable
		{
			get
			{
				return steerable_;
			}
			set
			{
				steerable_ = value;
			}
		}

		public float maxSteeringReduction
		{
			get
			{
				return maxSteeringReduction_;
			}
			set
			{
				maxSteeringReduction_ = value;
			}
		}

		public float maxSteeringReductionKeyboard => maxSteeringReductionKeyboard_;

		public float maxSteeringAngle
		{
			get
			{
				return maxSteeringAngle_ * maxSteeringMultiplier;
			}
			set
			{
				maxSteeringAngle_ = value;
			}
		}

		public float maxSteeringAngleKeyboard => maxSteeringAngleKeyboard_;

		public float steeringSpeed => steeringSpeed_;

		public float currentSteeringAngle
		{
			get;
			set;
		}

		public bool steeringStraight
		{
			get;
			set;
		}

		public float maxSteeringMultiplier
		{
			get;
			set;
		}

		public float steeringForceMultiplier
		{
			get
			{
				return steeringForceMultiplier_;
			}
			set
			{
				steeringForceMultiplier_ = value;
			}
		}

		public float distanceFactor
		{
			get;
			set;
		}

		public float travel
		{
			get;
			set;
		}

		public float wheelLoad
		{
			get;
			set;
		}

		public float forwardSlip
		{
			get;
			set;
		}

		public float sidewaysSlip
		{
			get;
			set;
		}

		public Vector3 sidewaysDir
		{
			get;
			set;
		}

		public bool antiRollForceApplied
		{
			get;
			set;
		}

		public float angularDamping => angularDamping_;

		public float downAngleForce => downAngleForce_;

		public float speedModifier => 1f;

		public bool isValid => grounded;

		public bool affectsMaxSpeed => true;

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

		public WheelComponentImplementor()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			hitNormal = Vector3.get_up();
			maxSteeringMultiplier = 1f;
			_zSide = WheelZSide.Front;
			positiveAxisMaxSpeed = Vector3.get_one();
			negativeAxisMaxSpeed = Vector3.get_one();
			minItemsModifier = 1f;
		}

		public void InitialiseMachineSide(WheelZSide machineSide_)
		{
			_zSide = machineSide_;
		}

		public void InitialiseMachineSide(WheelXSide machineSide_)
		{
			_xSide = machineSide_;
		}

		public void SetWheelColliderInfo(WheelColliderData data)
		{
			_wheelColliderData = data;
		}

		public void WheelColliderActivated()
		{
			ActivateColliders(_wheelColliderData, regularWheelColliderFriction);
		}

		private void ActivateColliders(WheelColliderData wheelData, WheelFriction trackFriction)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			if (wheelData.wheelCollider != null)
			{
				wheelData.wheelCollider.set_motorTorque(1E-06f);
				wheelData.wheelCollider.set_steerAngle(0f);
				wheelData.wheelCollider.set_forwardFriction(trackFriction.GetForwardWheelFrictionCurve(1f, 0f));
				wheelData.wheelCollider.set_sidewaysFriction(trackFriction.GetSidewaysWheelFrictionCurve(1f, 0f));
			}
		}
	}
}
