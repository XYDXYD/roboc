using Svelto.ECS;

namespace Simulation.GUI
{
	internal interface IHealthBarMachineIdComponent
	{
		int machineId
		{
			get;
			set;
		}

		DispatchOnSet<bool> isActive
		{
			get;
		}
	}
}
