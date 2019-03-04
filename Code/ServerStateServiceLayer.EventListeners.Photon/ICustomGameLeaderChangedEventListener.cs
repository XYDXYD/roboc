using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal interface ICustomGameLeaderChangedEventListener : IServerStateEventListener<string>, IServiceEventListener<string>, IServiceEventListenerBase
	{
	}
}
