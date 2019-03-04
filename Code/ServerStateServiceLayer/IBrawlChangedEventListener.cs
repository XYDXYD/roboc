using Svelto.ServiceLayer;

namespace ServerStateServiceLayer
{
	internal interface IBrawlChangedEventListener : IServerStateEventListener<bool>, IServiceEventListener<bool>, IServiceEventListenerBase
	{
	}
}
