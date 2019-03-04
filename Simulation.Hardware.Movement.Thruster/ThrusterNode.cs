using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class ThrusterNode : EntityView
	{
		public IHardwareOwnerComponent ownerComponent;

		public IFacingDirectionComponent facingComponent;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IThrusterForceComponent forceMagnitudeComponent;

		public IMaxSpeedComponent maxVelocityComponent;

		public IInputReceivedComponent inputReceivedComponent;

		public IThrusterParticleEffectsComponent particleEffects;

		public IVisibilityComponent visibilityComponent;

		public IMovementStoppingParamsComponent stoppingComponent;

		public IHardwareDisabledComponent disabledComponent;

		public IVerticalOrientationThrusterCountComponent verticalCountComponent;

		public IThrusterForceAppliedComponent forceAppliedComponent;

		public ITypeComponent typeComponent;

		public IAccelerationDelayComponent accelerationDelayComponent;

		public ISpeedModifierComponent speedModifierComponent;

		public ThrusterNode()
			: this()
		{
		}
	}
}
