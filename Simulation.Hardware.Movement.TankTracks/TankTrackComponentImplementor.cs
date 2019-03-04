using Svelto.DataStructures;
using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal class TankTrackComponentImplementor : MonoBehaviour, IMachineSideComponent, ITrackGroundedComponent, IPendingForceComponent, IWheelColliderDataComponent, ISteeringAngleComponent, IDistanceToCOMComponent, IMaxSpeedComponent, ILateralAccelerationComponent, ITurnAccelerationComponent, IMaxTurnRateMovingComponent, IMaxTurnRateStoppedComponent, IFrictionAngleComponent, ICurrentSlopeScalarComponent, IRegularCollidersParentComponent, ISupportCollidersParentComponent, IForcePointComponent, IWheelColliderInfo, IPartLevelComponent, IFrictionStiffnessComponent, ITrackStoppedComponent, ITrackTurningToDriveDirectionComponent, ISpeedModifierComponent, IMaxCarryMassComponent, IValidMovementComponent
	{
		public int level_ = 1;

		public float distanceToCentreLine;

		public float maxTurnRateMoving_ = 45f;

		public float maxTurnRateStopped_ = 135f;

		public float turnAcceleration_ = 1000f;

		public Transform forcePointTransform_;

		public float lateralAcceleration_ = 5f;

		public float angleWithMaxFriction_ = 60f;

		public float angleWithMinFriction_ = 75f;

		public float minFrictionScalar_;

		public float movingFrictionStiffness_ = 1f;

		public float stoppedFrictionStiffness_ = 0.85f;

		public GameObject regularWheelCollidersParent;

		public GameObject supportWheelCollidersParent;

		public TankTrackFriction regularWheelColliderFriction;

		public TankTrackFriction supportWheelColliderFriction;

		private FasterList<WheelColliderData> _allWheelColliders = new FasterList<WheelColliderData>(3);

		private FasterList<WheelColliderData> _supportWheelColliders = new FasterList<WheelColliderData>(6);

		private MachineSide _machineSide;

		public Vector3 localForceOffset
		{
			get;
			set;
		}

		public MachineSide machineSide
		{
			get
			{
				return _machineSide;
			}
			set
			{
				_machineSide = value;
			}
		}

		public bool grounded
		{
			get;
			set;
		}

		public int groundedWheelCount
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

		public FasterList<WheelColliderData> wheelColliders
		{
			get
			{
				if (regularWheelCollidersParent.get_activeSelf())
				{
					return _allWheelColliders;
				}
				return _supportWheelColliders;
			}
		}

		public float steeringAngle
		{
			get;
			set;
		}

		public Vector3 peviousCentreOfMass
		{
			get;
			set;
		}

		public float distance
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

		public float turnAcceleration => turnAcceleration_;

		public float maxTurnRateMoving => maxTurnRateMoving_;

		public float maxTurnRateStopped => maxTurnRateStopped_;

		public float angleWithMaxFriction => angleWithMaxFriction_;

		public float angleWithMinFriction => angleWithMinFriction_;

		public float minFrictionScalar => minFrictionScalar_;

		public float currentSlopeScalar
		{
			get;
			set;
		}

		public GameObject regularParent => regularWheelCollidersParent;

		public GameObject supportParent => supportWheelCollidersParent;

		public Transform forcePointTransform => forcePointTransform_;

		public Vector3 forcePoint
		{
			get;
			set;
		}

		public Vector3 forcePointOffset
		{
			get;
			set;
		}

		public int level => level_;

		public float stoppedFrictionStiffness => stoppedFrictionStiffness_;

		public float movingFrictionStiffness => movingFrictionStiffness_;

		public bool stopped
		{
			get;
			set;
		}

		public bool previousStoppedState
		{
			get;
			set;
		}

		public bool turning
		{
			get;
			set;
		}

		public bool previousTurningState
		{
			get;
			set;
		}

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

		public TankTrackComponentImplementor()
			: this()
		{
		}

		private void Awake()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			positiveAxisMaxSpeed = Vector3.get_one();
			negativeAxisMaxSpeed = Vector3.get_one();
			minItemsModifier = 1f;
		}

		public void InitialiseMachineSide(MachineSide machineSide_)
		{
			_machineSide = machineSide_;
		}

		public void SetWheelColliderInfo(WheelColliderData data)
		{
			if (!data.support)
			{
				_allWheelColliders.Add(data);
			}
			else
			{
				_supportWheelColliders.Add(data);
			}
		}

		public void WheelColliderActivated()
		{
			ActivateColliders(_allWheelColliders, regularWheelColliderFriction);
			ActivateColliders(_supportWheelColliders, supportWheelColliderFriction);
		}

		private void ActivateColliders(FasterList<WheelColliderData> wheelColliders, TankTrackFriction trackFriction)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < wheelColliders.get_Count(); i++)
			{
				WheelColliderData wheelColliderData = wheelColliders.get_Item(i);
				if (wheelColliderData.wheelCollider != null)
				{
					wheelColliderData.wheelCollider.set_motorTorque(1E-06f);
					wheelColliderData.wheelCollider.set_steerAngle(0f);
					wheelColliderData.wheelCollider.set_forwardFriction(trackFriction.GetForwardWheelFrictionCurve());
					wheelColliderData.wheelCollider.set_sidewaysFriction(trackFriction.GetSidewaysWheelFrictionCurve());
				}
			}
		}
	}
}
