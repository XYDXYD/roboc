using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IClanInviteCancelledEventListener : IServiceEventListener<ClanInvite[]>, IServiceEventListenerBase
	{
	}
}
