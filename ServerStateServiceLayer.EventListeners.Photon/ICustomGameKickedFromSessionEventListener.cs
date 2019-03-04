using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal interface ICustomGameKickedFromSessionEventListener : IServerStateEventListener<KickedFromCustomGameSessionData>, IServiceEventListener<KickedFromCustomGameSessionData>, IServiceEventListenerBase
	{
	}
}
