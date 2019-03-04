using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IPlayerChatStateEventListener : IServiceEventListener<PlayerChatStateUpdateData>, IServiceEventListenerBase
	{
	}
}
