using Svelto.ECS;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineSideComponent machineSideComponent;

		public ITrackGroundedComponent trackGroundedComponent;

		public IPendingForceComponent pendingForceComponent;

		public IWheelColliderDataComponent wheelColliderDataComponent;

		public IDistanceToCOMComponent distanceToCOMComponent;

		public IMaxSpeedComponent maxSpeedComponent;

		public ILateralAccelerationComponent lateralAccelerationComponent;

		public ITurnAccelerationComponent turnAccelerationComponent;

		public IMaxTurnRateMovingComponent maxTurnRateMovingComponent;

		public IMaxTurnRateStoppedComponent maxTurnRateStoppedComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IFrictionAngleComponent frictionAngleComponent;

		public ICurrentSlopeScalarComponent currentSlopeScalarComponent;

		public IForcePointComponent forcePointComponent;

		public IVisibilityComponent visibilityComponent;

		public IFrictionStiffnessComponent frictionStiffnessComponent;

		public ITrackStoppedComponent trackStoppedComponent;

		public ITrackTurningToDriveDirectionComponent turningToDriveDirection;

		public TankTrackNode()
			: this()
		{
		}
	}
}
