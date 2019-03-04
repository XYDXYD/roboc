using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IFriendRemovedEventListener : IServiceEventListener<FriendListUpdate>, IServiceEventListenerBase
	{
	}
}
