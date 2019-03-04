using Svelto.ECS;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal sealed class RemoteAerofoilGraphicsNode : EntityView
	{
		public IRigidBodyComponent rigidbodyComponent;

		public ITransformComponent transformComponent;

		public IVisibilityComponent visibilityComponent;

		public IAerofoilGFXComponent aerofoilGfxComponent;

		public IHardwareDisabledComponent disabledComponent;

		public RemoteAerofoilGraphicsNode()
			: this()
		{
		}
	}
}
