using Svelto.ECS;

namespace Simulation.Hardware.Movement.MechLegs
{
	internal sealed class MechLegMachineView : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IRigidBodyComponent rigidbodyComponent;

		public IMachineInputComponent inputComponent;

		public IMachineFunctionalComponent rectifyingComponent;

		public IMachineStunComponent stunComponent;

		public MechLegMachineView()
			: this()
		{
		}
	}
}
