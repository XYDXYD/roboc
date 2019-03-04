using Svelto.ECS;

namespace Simulation.Hardware.Movement.Aerofoil
{
	internal sealed class LocalMachineAerofoilNode : EntityView
	{
		public IMachineInputComponent inputComponent;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineAerofoilComponent aerofoilComponent;

		public IMachineStunComponent machineStunComponent;

		public LocalMachineAerofoilNode()
			: this()
		{
		}
	}
}
