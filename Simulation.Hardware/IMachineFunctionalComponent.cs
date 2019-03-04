using Svelto.ECS;

namespace Simulation.Hardware
{
	public interface IMachineFunctionalComponent
	{
		bool functionalsEnabled
		{
			get;
			set;
		}

		DispatchOnChange<bool> onFunctionalsEnabled
		{
			get;
		}
	}
}
