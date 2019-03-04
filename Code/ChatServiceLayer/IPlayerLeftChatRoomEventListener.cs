using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IPlayerLeftChatRoomEventListener : IServiceEventListener<PlayerLeftChatRoomData>, IServiceEventListenerBase
	{
	}
}
