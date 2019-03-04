using Services;
using Svelto.ServiceLayer;

namespace LobbyServiceLayer.Photon
{
	internal class LobbyEventContainerFactory : ILobbyEventContainerFactory, IEventContainerFactory
	{
		private readonly LobbyEventRegistry _lobbyEventRegistry;

		public LobbyEventContainerFactory(LobbyEventRegistry lobbyEventRegistry)
		{
			_lobbyEventRegistry = lobbyEventRegistry;
		}

		public IServiceEventContainer Create()
		{
			return new LobbyEventContainer(_lobbyEventRegistry);
		}
	}
}
