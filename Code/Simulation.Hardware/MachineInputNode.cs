using Svelto.ECS;

namespace Simulation.Hardware
{
	internal sealed class MachineInputNode : EntityView
	{
		public IMachineInputComponent machineInput;

		public IMachineOwnerComponent ownerComponent;

		public IInputMotorComponent motorForwardComponent;

		public MachineInputNode()
			: this()
		{
		}
	}
}
