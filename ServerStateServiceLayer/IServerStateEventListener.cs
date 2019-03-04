using Svelto.ServiceLayer;

namespace ServerStateServiceLayer
{
	internal interface IServerStateEventListener<T> : IServiceEventListener<T>, IServiceEventListenerBase
	{
	}
	internal interface IServerStateEventListener<T, U> : IServiceEventListener<T, U>, IServiceEventListenerBase
	{
	}
}
