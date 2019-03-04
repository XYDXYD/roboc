using UnityEngine;

namespace Simulation
{
	internal sealed class MechLegData
	{
		public bool initialised;

		public float massPerLeg;

		public Vector3 targetLegPosition = Vector3.get_zero();

		public float targetHeight = 3f;

		public Vector3 currentVelocity = Vector3.get_zero();

		public Vector3 previousVelocity = Vector3.get_zero();

		public float currentSpeedRatio;

		public float currentSpeed;

		public Vector3 lastPosition = Vector3.get_zero();

		public Vector3 slideStartVelocity = Vector3.get_zero();

		public bool longJumping;

		private bool _jumping;

		public bool stopped;

		public Vector3 stoppedPosition = Vector3.get_zero();

		public Vector3 vertHitNormal = Vector3.get_zero();

		public Vector3 footHitNormal = Vector3.get_zero();

		public float vertDistToGround = float.MaxValue;

		public bool terrainWalkingAngleValid;

		public bool terrainSlidingAngleValid;

		public bool legSliding;

		public bool legShouldSlide;

		public bool crouching;

		public bool groundedOnItself;

		private CubeFace _facingDirection;

		private bool _legGrounded;

		public bool _legLanding;

		private float _legGroundedTime;

		private float _legGroundedAfterLongJumpTime;

		private float _timeGroundedBeforeJump;

		private float _timeGroundedAfterJump;

		internal bool descending;

		public bool jumping
		{
			get
			{
				return _jumping;
			}
			set
			{
				_jumping = value;
				if (!_jumping)
				{
					longJumping = false;
					descending = false;
				}
			}
		}

		public bool xIsSideways
		{
			get;
			private set;
		}

		public CubeFace facingDirection
		{
			get
			{
				return _facingDirection;
			}
			set
			{
				_facingDirection = value;
				xIsSideways = (_facingDirection == CubeFace.Front || _facingDirection == CubeFace.Back);
			}
		}

		public bool legGrounded
		{
			get
			{
				return _legGrounded;
			}
			set
			{
				if (value && !_legGrounded)
				{
					_legGroundedTime = Time.get_time();
					if (longJumping)
					{
						_legGroundedAfterLongJumpTime = Time.get_time();
					}
				}
				_legGrounded = value;
			}
		}

		public bool legLanding
		{
			get
			{
				return _legLanding;
			}
			set
			{
				_legLanding = value;
			}
		}

		public bool canJump => _legGrounded && Time.get_time() - _legGroundedTime > _timeGroundedBeforeJump;

		public bool canMove => _legGrounded && Time.get_time() - _legGroundedAfterLongJumpTime > _timeGroundedAfterJump;

		public MechLegData(Transform t, float timeGroundedBeforeJump, float timeGroundedAfterJunmp)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			lastPosition = t.get_position();
			_timeGroundedBeforeJump = timeGroundedBeforeJump;
			_timeGroundedAfterJump = timeGroundedAfterJunmp;
		}
	}
}
