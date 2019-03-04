using Svelto.ECS;

namespace Simulation.Hardware.Movement.InsectLegs
{
	internal sealed class InsectLegMachineView : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineInputComponent inputComponent;

		public IMachineFunctionalComponent rectifyingComponent;

		public IMachineStunComponent stunComponent;

		public InsectLegMachineView()
			: this()
		{
		}
	}
}
