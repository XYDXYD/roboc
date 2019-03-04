using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.EventListeners.Photon
{
	internal interface ICustomGameDeclinedInvitationEventListener : IServerStateEventListener<DeclineInviteToSessionData>, IServiceEventListener<DeclineInviteToSessionData>, IServiceEventListenerBase
	{
	}
}
