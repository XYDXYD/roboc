using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IFriendDeclineEventListener : IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
	}
}
