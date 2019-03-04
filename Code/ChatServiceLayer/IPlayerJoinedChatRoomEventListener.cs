using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IPlayerJoinedChatRoomEventListener : IServiceEventListener<PlayerJoinedChatRoomData>, IServiceEventListenerBase
	{
	}
}
