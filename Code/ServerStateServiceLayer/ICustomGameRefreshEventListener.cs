using Svelto.ServiceLayer;

namespace ServerStateServiceLayer
{
	internal interface ICustomGameRefreshEventListener : IServerStateEventListener<string>, IServiceEventListener<string>, IServiceEventListenerBase
	{
	}
}
