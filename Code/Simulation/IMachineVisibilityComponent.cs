using Svelto.ECS.Legacy;

namespace Simulation
{
	public interface IMachineVisibilityComponent
	{
		bool isVisible
		{
			get;
			set;
		}

		Dispatcher<IMachineVisibilityComponent, int> machineBecameVisible
		{
			get;
		}

		Dispatcher<IMachineVisibilityComponent, int> machineBecameInvisible
		{
			get;
		}
	}
}
