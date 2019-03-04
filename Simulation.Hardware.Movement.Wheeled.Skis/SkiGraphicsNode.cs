using Svelto.ECS;

namespace Simulation.Hardware.Movement.Wheeled.Skis
{
	internal class SkiGraphicsNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineSideComponent machineSideComponent;

		public IHardwareDisabledComponent hardwareDisabledComponent;

		public IVisibilityComponent visibilityComponent;

		public ISteeringComponent steeringComponent;

		public IGraphicsTransformComponent graphicsTransformComponent;

		public IPreviousStateComponent previousStateComponent;

		public ISkiSuspensionComponent suspensionComponent;

		public ISkiRaycastComponent raycastComponent;

		public ISkiHingeComponent hingeComponent;

		public IGroundedComponent groundedComponent;

		public ISpeedComponent speedComponent;

		public SkiGraphicsNode()
			: this()
		{
		}
	}
}
