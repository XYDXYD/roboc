using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IFriendCancelledEventListener : IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
	}
}
