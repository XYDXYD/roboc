using Svelto.ECS.Legacy;

namespace Simulation
{
	internal interface IModuleActivationComponent
	{
		Dispatcher<IModuleActivationComponent, int> activate
		{
			get;
		}
	}
}
