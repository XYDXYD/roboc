using Svelto.ServiceLayer;

namespace ServerStateServiceLayer
{
	internal interface IMaintenanceModeEventListener : IServerStateEventListener<string>, IServiceEventListener<string>, IServiceEventListenerBase
	{
	}
}
