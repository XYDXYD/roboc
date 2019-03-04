using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal sealed class WheelGraphicsNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineSideComponent machineSideComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IVisibilityComponent visibilityComponent;

		public ISteeringComponent steeringComponent;

		public IPreviousStateComponent previousStateComponent;

		public IGraphicsTransformComponent graphicsTransformComponent;

		public IWheelSpeedComponent wheelSpeedComponent;

		public ILateralAccelerationComponent lateralAccelerationComponent;

		public IMaxSpeedComponent maxSpeedComponent;

		public IWheelRadiusComponent radiusComponent;

		public IWheelSuspensionComponent wheelSuspensionComponent;

		public IWheelRotationAxisComponent rotationAxisComponent;

		public IGroundedComponent groundedComponent;

		public IBrakeComponent brakeComponent;

		public WheelGraphicsNode()
			: this()
		{
		}
	}
}
