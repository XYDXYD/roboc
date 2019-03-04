using Svelto.ECS.Legacy;

namespace Simulation
{
	public interface IEmpStunActivationComponent
	{
		Dispatcher<IEmpStunActivationComponent, int> activateEmpStun
		{
			get;
		}
	}
}
