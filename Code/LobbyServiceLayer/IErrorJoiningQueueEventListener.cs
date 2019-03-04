using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IErrorJoiningQueueEventListener : IServiceEventListener<LobbyReasonCode>, IServiceEventListenerBase
	{
	}
}
