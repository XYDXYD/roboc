using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal class ClearPlatoonCacheCacheRequest : IClearPlatoonCacheRequest, IServiceRequest
	{
		public void Execute()
		{
			if (CacheDTO.platoon.isInPlatoon)
			{
				CacheDTO.platoon = null;
			}
		}
	}
}
