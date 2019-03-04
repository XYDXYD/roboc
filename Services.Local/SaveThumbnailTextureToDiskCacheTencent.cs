using Svelto.ServiceLayer;
using System.IO;
using UnityEngine;

namespace Services.Local
{
	internal class SaveThumbnailTextureToDiskCacheTencent : ISaveThumbnailTextureToDiskCache, IServiceRequest<SaveTextureToDiskCacheDependency>, IServiceRequest
	{
		private SaveTextureToDiskCacheDependency _params;

		public void Inject(SaveTextureToDiskCacheDependency dependency)
		{
			_params = dependency;
		}

		public void Execute()
		{
			if (!Directory.Exists(TextureCacheHelper.GetCacheFolder()))
			{
				Directory.CreateDirectory(TextureCacheHelper.GetCacheFolder());
			}
			CacheDTO.robotShopThumbnails.Add(_params.textureURL, _params.texture);
			string cachePath = TextureCacheHelper.GetCachePath("/" + _params.textureURL);
			File.WriteAllBytes(cachePath, ImageConversion.EncodeToPNG(_params.texture));
		}
	}
}
