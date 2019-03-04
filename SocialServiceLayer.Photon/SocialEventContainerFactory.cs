using Services;
using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal class SocialEventContainerFactory : ISocialEventContainerFactory, IEventContainerFactory
	{
		private SocialEventRegistry _socialEventRegistry;

		public SocialEventContainerFactory(SocialEventRegistry socialEventRegistry)
		{
			_socialEventRegistry = socialEventRegistry;
		}

		public IServiceEventContainer Create()
		{
			return new SocialEventContainer(_socialEventRegistry);
		}
	}
}
