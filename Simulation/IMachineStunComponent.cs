using Svelto.ECS.Legacy;

namespace Simulation
{
	public interface IMachineStunComponent
	{
		Dispatcher<IMachineStunComponent, int> machineStunned
		{
			get;
		}

		Dispatcher<IMachineStunComponent, int> remoteMachineStunned
		{
			get;
		}

		float stunTimer
		{
			get;
			set;
		}

		bool stunned
		{
			get;
			set;
		}

		int stunningEmpLocator
		{
			get;
			set;
		}
	}
}
