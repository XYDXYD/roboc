using Svelto.ECS;

namespace Simulation.Hardware.Movement.TankTracks
{
	internal sealed class TankTrackMachineNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineStoppedComponent machineStoppedComponent;

		public IMovingBackwardsComponent movingBackwardsComponent;

		public IAvgMaxSpeedComponent avgMaxSpeedComponent;

		public IAvgLateralAccelerationComponent avgLateralAccelerationComponent;

		public IAvgTurnAccelerationComponent avgTurnAccelerationComponent;

		public IAvgMaxTurnRateMovingComponent avgMaxTurnRateMovingComponent;

		public IAvgMaxTurnRateStoppedComponent avgMaxTurnRateStoppedComponent;

		public ICacheUpdateComponent cacheUpdateComponent;

		public INumGroundedTracksComponent numGroundedTracksComponent;

		public IDesiredTurningTorqueComponent desiredTurningTorqueComponent;

		public IFastSideTurningTorqueComponent fastSideTurningTorqueComponent;

		public IStrafingCustomAngleToStraightComponent strafingCustomAngleToStraightComponent;

		public IStrafingCustomInputComponent strafingCustomInputComponent;

		public IVisibilityComponent visibilityComponent;

		public IMachineTurningToDriveDirectionComponent turningToDriveDirection;

		public TankTrackMachineNode()
			: this()
		{
		}
	}
}
