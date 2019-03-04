using Svelto.ECS.Legacy;

namespace Simulation
{
	internal interface IModuleConfirmActivationComponent
	{
		Dispatcher<IModuleConfirmActivationComponent, int> activationConfirmed
		{
			get;
		}
	}
}
