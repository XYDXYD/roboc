using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IFriendAcceptEventListener : IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
	}
}
