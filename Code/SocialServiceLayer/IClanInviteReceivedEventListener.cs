using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IClanInviteReceivedEventListener : IServiceEventListener<ClanInvite, ClanInvite[]>, IServiceEventListenerBase
	{
	}
}
