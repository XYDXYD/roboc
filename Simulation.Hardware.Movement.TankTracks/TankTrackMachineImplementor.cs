using UnityEngine;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal class TankTrackMachineImplementor : IMachineStoppedComponent, IMovingBackwardsComponent, IAvgMaxSpeedComponent, IAvgLateralAccelerationComponent, IAvgTurnAccelerationComponent, IAvgMaxTurnRateMovingComponent, IAvgMaxTurnRateStoppedComponent, ICacheUpdateComponent, INumGroundedTracksComponent, IDesiredTurningTorqueComponent, IFastSideTurningTorqueComponent, IStrafingCustomAngleToStraightComponent, IStrafingCustomInputComponent, IVisibilityTracker, IVisibilityComponent, IMachineTurningToDriveDirectionComponent
	{
		private bool _offScreen;

		private bool _inRange;

		public bool stopped
		{
			get;
			set;
		}

		public bool movingBackwards
		{
			get;
			set;
		}

		public float maxSpeed
		{
			get;
			set;
		}

		public float acceleration
		{
			get;
			set;
		}

		public float turnAcceleration
		{
			get;
			set;
		}

		public float maxTurnRateMoving
		{
			get;
			set;
		}

		public float maxTurnRateStopped
		{
			get;
			set;
		}

		public bool updateRequired
		{
			get;
			set;
		}

		public int groundedTracks
		{
			get;
			set;
		}

		public Vector3 desiredTorque
		{
			get;
			set;
		}

		public Vector3 fastSideTorque
		{
			get;
			set;
		}

		public bool customAngleUsed
		{
			get;
			set;
		}

		public float angleToStraight
		{
			get;
			set;
		}

		public float forwardInput
		{
			get;
			set;
		}

		public float strafingInput
		{
			get;
			set;
		}

		public float turningInput
		{
			get;
			set;
		}

		public bool turning
		{
			get;
			set;
		}

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
	}
}
