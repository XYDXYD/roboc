using Svelto.ServiceLayer;

namespace LobbyServiceLayer
{
	internal interface IStartConnectionTestEventListener : IServiceEventListener<Host, NetworkConfig>, IServiceEventListenerBase
	{
	}
}
