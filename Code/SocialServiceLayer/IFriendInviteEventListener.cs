using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IFriendInviteEventListener : IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
	}
}
