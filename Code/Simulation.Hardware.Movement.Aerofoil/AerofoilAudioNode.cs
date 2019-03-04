using Svelto.ECS;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal sealed class AerofoilAudioNode : EntityView
	{
		public IAerofoilAFXComponent aerofoilAfxComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public ITransformComponent transformComponent;

		public IVisibilityComponent visibilityComponent;

		public IHardwareDisabledComponent disabledComponent;

		public AerofoilAudioNode()
			: this()
		{
		}
	}
}
