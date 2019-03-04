using Svelto.ECS;

namespace Simulation.Hardware.Movement.Hovers
{
	internal class MachineHoverNode : EntityView
	{
		public IMachineInputComponent inputComponent;

		public ITransformComponent transformComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineStunComponent machineStunComponent;

		public IAudioGameObjectComponent audioGOComponent;

		public IMachineOwnerComponent owner;

		public IMachineFunctionalComponent functionalComponent;

		public IMachineHoverGFXComponent effectsComponent;

		public MachineHoverNode()
			: this()
		{
		}
	}
}
