using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal interface ICustomGameConfigChangedEventListener : IServerStateEventListener<CustomGameConfigChangedData>, IServiceEventListener<CustomGameConfigChangedData>, IServiceEventListenerBase
	{
	}
}
