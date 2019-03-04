using Svelto.ECS;

namespace Simulation.Hardware.Movement.Thruster
{
	internal sealed class MachineThrusterView : EntityView
	{
		public IMachineOwnerComponent ownerComponent;

		public IMachineInputComponent inputComponent;

		public IMachineStunComponent stunComponent;

		public IMachineFunctionalComponent rectifierComponent;

		public MachineThrusterView()
			: this()
		{
		}
	}
}
