using UnityEngine;

namespace Simulation.Hardware.Cosmetic.Balloon
{
	internal class BalloonImplementor : MonoBehaviour, IBalloonComponent, IVisibilityComponent, IVisibilityTracker, IImplementor
	{
		public Transform[] Joints;

		public float SpringStength;

		public float Damping;

		public float LateralInfluence;

		public float AngularInfluence;

		private Vector3 _lastPos;

		private float _lastTime;

		private Vector3 _lastVelocity;

		private Vector3 _lastRotationEuler;

		private Vector3 _stalkRotationVelocity;

		private bool _offScreen;

		private bool _inRange;

		public bool isOffScreen
		{
			get
			{
				return _offScreen;
			}
			set
			{
				_offScreen = value;
			}
		}

		public bool isInRange
		{
			get
			{
				return _inRange;
			}
			set
			{
				_inRange = value;
			}
		}

		public bool offScreen => _offScreen;

		public bool inRange => _inRange;

		public Transform[] joints => Joints;

		public float springStength => SpringStength;

		public float damping => Damping;

		public float lateralInfluence => LateralInfluence;

		public float angularInfluence => AngularInfluence;

		public Vector3 lastPos
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _lastPos;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_lastPos = value;
			}
		}

		public float lastTime
		{
			get
			{
				return _lastTime;
			}
			set
			{
				_lastTime = value;
			}
		}

		public Vector3 lastVelocity
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _lastVelocity;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_lastVelocity = value;
			}
		}

		public Vector3 lastRotationEuler
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _lastRotationEuler;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_lastRotationEuler = value;
			}
		}

		public Vector3 stalkRotationVelocity
		{
			get
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return _stalkRotationVelocity;
			}
			set
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				_stalkRotationVelocity = value;
			}
		}

		public BalloonImplementor()
			: this()
		{
		}
	}
}
