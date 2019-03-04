using Svelto.ServiceLayer;
using System.IO;
using UnityEngine;

namespace Services.Local
{
	internal class LoadThumbnailTextureFromDiskCache : ILoadThumbnailTextureFromDiskCache, IServiceRequest<LoadThumbnailFromDiskCacheDependency>, IAnswerOnComplete<Texture2D>, IServiceRequest
	{
		private LoadThumbnailFromDiskCacheDependency _params;

		private IServiceAnswer<Texture2D> _answer;

		private ServiceBehaviour _serviceBehaviour;

		public LoadThumbnailTextureFromDiskCache()
		{
			_serviceBehaviour = new ServiceBehaviour("strGenericError", "strGenericErrorQuit");
		}

		public void Inject(LoadThumbnailFromDiskCacheDependency dependency)
		{
			_params = dependency;
		}

		public IServiceRequest SetAnswer(IServiceAnswer<Texture2D> answer)
		{
			_answer = answer;
			return this;
		}

		public void Execute()
		{
			if (!Directory.Exists(TextureCacheHelper.GetCacheFolder()))
			{
				Directory.CreateDirectory(TextureCacheHelper.GetCacheFolder());
			}
			if (CacheDTO.robotShopThumbnails.ContainsKey(_params.textureURL))
			{
				if (_answer != null && _answer.succeed != null)
				{
					_answer.succeed(CacheDTO.robotShopThumbnails.get_Item(_params.textureURL));
				}
			}
			else
			{
				LoadFromDiskIntoCacheIfExists();
			}
		}

		private void LoadFromDiskIntoCacheIfExists()
		{
			string cachePath = TextureCacheHelper.GetCachePath("/" + _params.textureURL);
			if (File.Exists(cachePath))
			{
				byte[] array = File.ReadAllBytes(cachePath);
				Texture2D val = GameUtility.CreateRobotShopTexture();
				bool flag = ImageConversion.LoadImage(val, array);
				CacheDTO.robotShopThumbnails.Add(_params.textureURL, val);
				if (flag && _answer != null && _answer.succeed != null)
				{
					_answer.succeed(val);
				}
			}
			else
			{
				_answer.failed(_serviceBehaviour);
			}
		}
	}
}
