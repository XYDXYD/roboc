using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled
{
	internal sealed class WheeledMachineNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public ISpeedComponent speedComponent;

		public IAccelerationComponent accelerationComponent;

		public IBrakingnComponent brakingComponent;

		public IWheelCacheUpdateComponent cacheUpdateComponent;

		public INumGroundedWheelsComponent numGroundedWheelsComponent;

		public IAngularVelocityComponent angualarVelocityComponent;

		public IStrafingCustomAngleToStraightComponent strafingCustomAngleToStraightComponent;

		public IStrafingCustomInputComponent strafingCustomInputComponent;

		public IInputHistoryComponent inputHistoryComponent;

		public IThreadSafeTransformComponent transformComponentThreadSafe;

		public IThreadSafeRigidBodyComponent rigidbodyComponentThreadSafe;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidBodyComponent;

		public IMachineFunctionalComponent machineRectifierComponent;

		public WheeledMachineNode()
			: this()
		{
		}
	}
}
