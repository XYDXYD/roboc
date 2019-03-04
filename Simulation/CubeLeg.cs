using RootMotion.FinalIK;
using Simulation.Hardware.Movement;
using Simulation.Hardware.Movement.InsectLegs;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class CubeLeg : MonoBehaviour, IVisibilityTracker, IInsectLegComponent, IMaxSpeedComponent, ISpeedModifierComponent, IMaxCarryMassComponent, IValidMovementComponent
	{
		internal bool wasHidden;

		public bool repositionFoot;

		public float maxLegLength = 2f;

		public float footOffsetToLegBase = 1.2f;

		public float lightLegMass = 100f;

		public LegMovementData lightLegMovement;

		public float heavyLegMass = 200f;

		public LegMovementData heavyLegMovement;

		public float validLegRadius = 0.8f;

		public float centeredLegRadius = 0.4f;

		public float timeGroundedBeforeJump = 0.5f;

		public float stoppedVelocityScale = 0.1f;

		public float animationHeight = 0.4f;

		public float animationTimeFastStep = 0.2f;

		public float animationTimeSlowStep = 0.4f;

		public Transform retractedPositionTransform;

		public AnimationCurve footHeightCurve;

		public AnimationCurve footMovementCurve;

		public Transform footTransform;

		public Transform actualFootPosition;

		public Vector3 swaggerDirection = new Vector3(0f, 1f, 0.5f);

		public int tier = 1;

		internal Transform T;

		private Vector3 _footOffset = Vector3.get_zero();

		private IKSolver _ikSolver;

		private Vector3 _targetIKPosition = Vector3.get_zero();

		private bool _intialised;

		[Inject]
		internal LegController legController
		{
			private get;
			set;
		}

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

		internal Vector3 retractedPosition => retractedPositionTransform.get_position();

		internal LegData legData
		{
			get;
			private set;
		}

		internal LegGraphics legGraphics
		{
			get;
			private set;
		}

		internal LegMovement movement
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

		public bool isValid => legData.legGrounded;

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
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_001f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				_targetIKPosition = value;
				_ikSolver.SetIKPosition(value - T.get_rotation() * _footOffset);
				SetFootRotation();
			}
		}

		internal Vector3 exactFootPosition => actualFootPosition.get_position();

		public CubeLeg()
			: this()
		{
		}//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)


		private void Awake()
		{
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			T = this.GetComponent<Transform>();
			legData = new LegData(T, timeGroundedBeforeJump);
			legGraphics = new LegGraphics();
			movement = new LegMovement(this);
			ik = this.GetComponentInChildren<IK>();
			_ikSolver = ik.GetIKSolver();
			_footOffset = Quaternion.Inverse(T.get_rotation()) * (exactFootPosition - footTransform.get_position());
			positiveAxisMaxSpeed = Vector3.get_one();
			negativeAxisMaxSpeed = Vector3.get_one();
			minItemsModifier = 1f;
		}

		private void Update()
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (repositionFoot)
			{
				repositionFoot = false;
				footPosition = footPosition;
			}
			SetFootRotation();
		}

		private void SetFootRotation()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			Transform obj = footTransform;
			Quaternion localRotation = footTransform.get_localRotation();
			Vector3 eulerAngles = localRotation.get_eulerAngles();
			obj.set_localRotation(Quaternion.Euler(eulerAngles.x, 0f, 0f));
			Vector3 val = Quaternion.Inverse(footTransform.get_rotation()) * (_targetIKPosition - footTransform.get_position());
			val.x = 0f;
			val.Normalize();
			float num = Vector3.Dot(Vector3.get_forward(), val);
			float num2 = Vector3.Dot(Vector3.get_up(), val);
			float num3 = Mathf.Acos(num) * 57.29578f * Mathf.Sign(num2);
			if (Mathf.Abs(num3) > 1f)
			{
				footTransform.Rotate(Vector3.get_left(), num3, 1);
			}
		}

		internal void UpdateCachedValues()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			position = T.get_position();
			Quaternion rotation = T.get_rotation();
			this.rotation = rotation.get_eulerAngles();
			up = T.get_forward();
			down = -T.get_forward();
			right = -T.get_right();
			left = T.get_right();
			forward = T.get_up();
			back = -T.get_up();
			forcePoint = position + forward * footOffsetToLegBase;
		}

		internal void UpdateForcePoint(Vector3 worldCenterOfMass)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = worldCenterOfMass - position;
			float num = Vector3.Dot(val, up);
			forcePoint = position + forward * footOffsetToLegBase + up * num;
		}

		private void OnDrawGizmos()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			if (legData != null && legData.legGrounded)
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
				Gizmos.DrawSphere(legGraphics.targetPos, 0.08f);
				Gizmos.set_color(new Color(1f, 0.5f, 0f));
				Gizmos.DrawSphere(_targetIKPosition, 0.05f);
				Gizmos.set_color(new Color(0f, 0f, 1f, 0.05f));
				Vector3 val = position + forward * footOffsetToLegBase + down * movement.idealHeight;
				Gizmos.DrawSphere(val, validLegRadius);
			}
		}
	}
}
