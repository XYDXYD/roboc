using Svelto.ServiceLayer;

namespace Services
{
	internal interface IEventContainerFactory
	{
		IServiceEventContainer Create();
	}
}
