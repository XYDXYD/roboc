using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled
{
	internal sealed class WheelNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineSideComponent machineSideComponent;

		public IGroundedComponent groundedComponent;

		public IPendingForceComponent pendingForceComponent;

		public IWheelColliderDataComponent wheelColliderDataComponent;

		public IMaxSpeedComponent maxSpeedComponent;

		public ILateralAccelerationComponent lateralAccelerationComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IFrictionAngleComponent frictionAngleComponent;

		public ICurrentSlopeScalarComponent currentSlopeScalarComponent;

		public IForcePointComponent forcePointComponent;

		public IVisibilityComponent visibilityComponent;

		public IFrictionStiffnessComponent frictionStiffnessComponent;

		public ISteeringComponent steeringComponent;

		public ICOMDistanceRangeComponent comDistanceComponent;

		public IBrakeComponent brakeComponent;

		public IAntirollComponent antirollComponent;

		public IWheelLoadComponent wheelLoadComponent;

		public ISlipComponent slipComponent;

		public IAngularDampingComponent angularDampingComponent;

		public IDownForceComponent downForceComponent;

		public IGridLocationComponent gridLocationComponent;

		public IPartLevelComponent levelComponent;

		public IThreadSafeTransformComponent transformComponentThreadSafe;

		public IThreadSafeWheelComponent threadSafeWheelComponent;

		public WheelNode()
			: this()
		{
		}
	}
}
