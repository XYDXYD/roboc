using Simulation.Hardware;

namespace Simulation
{
	internal class MachineGroundedImplementor : IMachineGroundedComponent
	{
		bool IMachineGroundedComponent.grounded
		{
			get;
			set;
		}
	}
}
