using Svelto.ServiceLayer;

namespace ServerStateServiceLayer
{
	internal interface IDevMessageEventListener : IServerStateEventListener<DevMessage>, IServiceEventListener<DevMessage>, IServiceEventListenerBase
	{
	}
}
