using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IFriendClanChangedEventListener : IServiceEventListener<FriendClanChangedEventArgs>, IServiceEventListenerBase
	{
	}
}
