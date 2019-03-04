namespace Simulation.Hardware.Movement.Wheeled
{
	internal class WheeledMachineImplementor : ISpeedComponent, IAccelerationComponent, IBrakingnComponent, IWheelCacheUpdateComponent, INumGroundedWheelsComponent, IStrafingCustomAngleToStraightComponent, IStrafingCustomInputComponent, IAngularVelocityComponent, IInputHistoryComponent
	{
		public float currentSpeed
		{
			get;
			set;
		}

		public bool movingBackwards
		{
			get;
			set;
		}

		public float avgAcceleration
		{
			get;
			set;
		}

		public float avgSteeringForceMultiplier
		{
			get;
			set;
		}

		public float accelerationScaler
		{
			get;
			set;
		}

		public float avgTimeToMaxAcceleration
		{
			get;
			set;
		}

		public bool updateRequired
		{
			get;
			set;
		}

		public bool updateSteeringRequired
		{
			get;
			set;
		}

		public int groundedParts
		{
			get;
			set;
		}

		public int groundedMotorizedParts
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

		public float avgBrake
		{
			get;
			set;
		}

		public float prevForwardInput
		{
			get;
			set;
		}

		public float prevRightAngularSpeed
		{
			get;
			set;
		}

		public float avgAngularDamping
		{
			get;
			set;
		}

		public WheeledMachineImplementor()
		{
			updateSteeringRequired = true;
		}
	}
}
