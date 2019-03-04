using Svelto.ECS;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal sealed class LocalAerofoilGraphicsNode : EntityView
	{
		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineStunComponent machineStunComponent;

		public IAerofoilGFXComponent aerofoilGfxComponent;

		public IHardwareDisabledComponent disabledComponent;

		public LocalAerofoilGraphicsNode()
			: this()
		{
		}
	}
}
