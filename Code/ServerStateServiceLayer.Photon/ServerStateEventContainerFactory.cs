using Services;
using Svelto.ServiceLayer;

namespace ServerStateServiceLayer.Photon
{
	internal class ServerStateEventContainerFactory : IServerStateEventContainerFactory, IEventContainerFactory
	{
		private readonly ServerStateEventRegistry _serverStateEventRegistry;

		public ServerStateEventContainerFactory(ServerStateEventRegistry serverStateEventRegistry)
		{
			_serverStateEventRegistry = serverStateEventRegistry;
		}

		public IServiceEventContainer Create()
		{
			return new ServerStateEventContainer(_serverStateEventRegistry);
		}
	}
}
