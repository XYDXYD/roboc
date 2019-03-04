using Services;
using Svelto.ServiceLayer;

namespace SinglePlayerServiceLayer.Photon
{
	internal class SinglePlayerEventContainerFactory : ISinglePlayerEventContainerFactory, IEventContainerFactory
	{
		private SinglePlayerEventRegistry _singlePlayerEventRegistry;

		public SinglePlayerEventContainerFactory(SinglePlayerEventRegistry singlePlayerEventRegistry)
		{
			_singlePlayerEventRegistry = singlePlayerEventRegistry;
		}

		public IServiceEventContainer Create()
		{
			return new SinglePlayerEventContainer(_singlePlayerEventRegistry);
		}
	}
}
