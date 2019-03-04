using Svelto.ECS;

namespace Simulation.Hardware.Movement.Rotors
{
	internal sealed class MachineRotorGraphicsNode : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRotorInputComponent inputComponent;

		public IRotorPowerValueComponent powerValueComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineStunComponent machineStunComponent;

		public MachineRotorGraphicsNode()
			: this()
		{
		}
	}
}
