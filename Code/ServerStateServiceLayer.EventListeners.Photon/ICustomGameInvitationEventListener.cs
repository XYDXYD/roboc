using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal interface ICustomGameInvitationEventListener : IServerStateEventListener<CustomGameInvitationData>, IServiceEventListener<CustomGameInvitationData>, IServiceEventListenerBase
	{
	}
}
