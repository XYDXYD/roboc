using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled.Wheels
{
	internal sealed class WheelAudioNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IGroundedComponent groundedComponent;

		public IMaxSpeedComponent maxSpeedComponent;

		public ILateralAccelerationComponent lateralAccelerationComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IVisibilityComponent visibilityComponent;

		public IWheelSpeedComponent wheelSpeedComponent;

		public IWheelLoadComponent wheelLoadComponent;

		public IPartLevelComponent levelComponent;

		public ISlipComponent slipComponent;

		public WheelAudioNode()
			: this()
		{
		}
	}
}
