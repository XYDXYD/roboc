using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer
{
	internal interface ISinglePlayerEventListener : IServiceEventListener, IServiceEventListenerBase
	{
	}
	internal interface ISinglePlayerEventListener<T> : IServiceEventListener<T>, IServiceEventListenerBase
	{
	}
}
