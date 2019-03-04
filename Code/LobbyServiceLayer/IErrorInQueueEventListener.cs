using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IErrorInQueueEventListener : IServiceEventListener<LobbyReasonCode>, IServiceEventListenerBase
	{
	}
}
