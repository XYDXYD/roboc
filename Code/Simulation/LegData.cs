using UnityEngine;

namespace Simulation
{
	internal sealed class LegData
	{
		public bool initialised;

		public float massPerLeg;

		public Vector3 targetLegPosition = Vector3.get_zero();

		public float targetHeight = 1f;

		public Vector3 currentVelocity = Vector3.get_zero();

		public float currentSpeed;

		public Vector3 lastPosition = Vector3.get_zero();

		public bool jumping;

		public bool stopped;

		public Vector3 stoppedPosition = Vector3.get_zero();

		public Vector3 stoppedRbPosition = Vector3.get_zero();

		public bool xInputFlipped;

		private CubeFace _facingDirection;

		private bool _legGrounded;

		private float _legGroundedTime;

		private float _timeGroundedBeforeJump;

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
				}
				_legGrounded = value;
			}
		}

		public bool canJump => _legGrounded && Time.get_time() - _legGroundedTime > _timeGroundedBeforeJump;

		public LegData(Transform t, float timeGroundedBeforeJump)
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
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			lastPosition = t.get_position();
			_timeGroundedBeforeJump = timeGroundedBeforeJump;
		}
	}
}
