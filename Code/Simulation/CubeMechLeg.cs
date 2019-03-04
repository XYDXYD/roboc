using RootMotion.FinalIK;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Movement.MechLegs;
using UnityEngine;

namespace Simulation
{
	internal sealed class CubeMechLeg : MonoBehaviour, IVisibilityTracker, IMechLegComponent, IMaxSpeedComponent, ISpeedModifierComponent, IMaxCarryMassComponent, IValidMovementComponent
	{
		public enum ColliderType
		{
			Walking,
			Falling,
			AlignmentRectifier,
			count
		}

		internal bool wasHidden;

		public float maxLegLength = 2f;

		public MechLegMovementData legMovement;

		public float validLegRadius = 0.8f;

		public float percentageOfStrideLength = 1.8f;

		public float percentageOfForwardStride = 1f;

		public float supportRadius = 1.6f;

		public float timeGroundedBeforeJump = 0.5f;

		public float timeGroundedAfterJump;

		public Vector3 updatedForcePoint = Vector3.get_zero();

		public float animationHeight = 0.4f;

		public float runningAnimationHeight;

		public float animationTimeFastStep = 0.2f;

		public float animationTimeSlowStep = 0.4f;

		public float startOfDropTime = 0.8f;

		public Transform jumpingRetractedPositionTransform;

		public Transform fallingRetractedPositionTransform;

		public Transform longJumpingRetractedPositionTransform;

		public Transform longJumpingFallingRetractedPositionTransform;

		public AnimationCurve footHeightCurve2;

		public AnimationCurve footHeightCurve;

		public AnimationCurve footMovementCurve;

		public Transform backLegTransform;

		public Transform footIKTransform;

		public Transform actualFootTransform;

		public Transform hipTargetTransform;

		public GameObject walkingCollider;

		internal SphereCollider[] colliders;

		public int tier = 1;

		public MechLegEffects legEffects;

		internal Transform T;

		private Vector3 _targetIKPosition = Vector3.get_zero();

		private Vector3 _hipDirection = Vector3.get_zero();

		public float tiltForce;

		private BaseMovementImplementor _baseImplementor;

		internal Vector3 position
		{
			get;
			private set;
		}

		internal Vector3 forcePoint
		{
			get;
			private set;
		}

		internal Vector3 rotation
		{
			get;
			private set;
		}

		internal Vector3 up
		{
			get;
			private set;
		}

		internal Vector3 down
		{
			get;
			private set;
		}

		internal Vector3 right
		{
			get;
			private set;
		}

		internal Vector3 left
		{
			get;
			private set;
		}

		internal Vector3 forward
		{
			get;
			private set;
		}

		internal Vector3 back
		{
			get;
			private set;
		}

		internal Vector3 jumpingRetractedPosition => jumpingRetractedPositionTransform.get_position();

		internal Vector3 fallingRetractedPosition => fallingRetractedPositionTransform.get_position();

		internal MechLegData legData
		{
			get;
			private set;
		}

		internal MechLegGraphics legGraphics
		{
			get;
			private set;
		}

		internal MechLegMovement movement
		{
			get;
			private set;
		}

		internal IK ik
		{
			get;
			private set;
		}

		public bool isOffScreen
		{
			internal get;
			set;
		}

		public bool isInRange
		{
			internal get;
			set;
		}

		public bool isHidden => isOffScreen && !isInRange;

		internal Vector3 footPosition
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _targetIKPosition;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				_targetIKPosition = value;
				footIKTransform.set_position(_targetIKPosition);
			}
		}

		internal Vector3 hipDirection
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _hipDirection;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				_hipDirection = value;
				hipTargetTransform.set_position(position + _hipDirection);
			}
		}

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

		public float speedModifier
		{
			get
			{
				if (legData.crouching && legData.legGrounded)
				{
					return legMovement.crouchingSpeedScale;
				}
				if (legData.longJumping && !legData.descending)
				{
					return legMovement.longJumpSpeedScale;
				}
				if (legData.legGrounded && !legData.canMove)
				{
					return 0.01f;
				}
				return 1f;
			}
		}

		public bool isValid => (legData.legGrounded && !legData.jumping) || (legData.longJumping && !legData.descending);

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

		public int minRequiredItems => _baseImplementor.minRequiredItems;

		public float minItemsModifierVal => _baseImplementor.minRequiredItemsModifier;

		public CubeMechLeg()
			: this()
		{
		}//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)


		private void OnValidate()
		{
		}

		private void Awake()
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			T = this.GetComponent<Transform>();
			legData = new MechLegData(T, timeGroundedBeforeJump, timeGroundedAfterJump);
			legGraphics = new MechLegGraphics();
			movement = new MechLegMovement(this);
			_baseImplementor = this.GetComponent<BaseMovementImplementor>();
			ik = this.GetComponentInChildren<IK>();
			colliders = walkingCollider.GetComponentsInChildren<SphereCollider>();
			colliders[1].set_enabled(false);
			colliders[2].set_enabled(false);
			positiveAxisMaxSpeed = Vector3.get_one();
			negativeAxisMaxSpeed = Vector3.get_one();
			minItemsModifier = 1f;
			legEffects.InitialiseLegEffects(actualFootTransform, backLegTransform);
		}

		internal void UpdateCachedValues()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			position = T.get_position();
			Quaternion rotation = T.get_rotation();
			this.rotation = rotation.get_eulerAngles();
			up = -T.get_up();
			down = T.get_up();
			right = -T.get_right();
			left = T.get_right();
			forward = -T.get_forward();
			back = T.get_forward();
			forcePoint = position;
		}

		private void OnDrawGizmos()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0103: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			if (legData != null)
			{
				Gizmos.set_color(Color.get_yellow());
				Gizmos.DrawSphere(legData.targetLegPosition, 0.1f);
				if (legGraphics.isAnimating)
				{
					Gizmos.set_color(Color.get_green());
				}
				else
				{
					Gizmos.set_color(Color.get_red());
				}
				Gizmos.DrawSphere(legGraphics.targetFootPos, 0.25f);
				Gizmos.set_color(new Color(0f, 0f, 1f, 0.15f));
				Vector3 val = position + down * movement.idealHeight;
				Gizmos.DrawSphere(val, validLegRadius);
				Gizmos.set_color(new Color(1f, 0f, 0f, 0.15f));
				Gizmos.DrawSphere(val, validLegRadius * 2f);
				Gizmos.set_color(Color.get_magenta());
				Gizmos.DrawSphere(footIKTransform.get_position(), 0.2f);
				Gizmos.set_color(Color.get_blue());
				Gizmos.DrawSphere(hipTargetTransform.get_position(), 0.25f);
				Gizmos.set_color(Color.get_green());
				Gizmos.DrawSphere(position, 0.25f);
			}
		}
	}
}
