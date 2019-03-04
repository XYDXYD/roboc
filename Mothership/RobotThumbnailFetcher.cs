using Mothership.Garage.Thumbnail;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using UnityEngine;

namespace Mothership
{
	internal class RobotThumbnailFetcher
	{
		[Inject]
		internal RobotShopObserver observer
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public void LoadTexture(string url, Action<Texture2D> onTextureLoaded, ThumbnailType typeToLoad = ThumbnailType.Shop)
		{
			LoadThumbnailFromDiskCacheDependency loadThumbnailFromDiskCacheDependency = new LoadThumbnailFromDiskCacheDependency();
			string[] array = url.Split('/');
			string text = string.Empty;
			if (array.Length == 1)
			{
				text = url;
			}
			else
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					text = ((i != array.Length - 2) ? (text + array[i] + "/") : (text + array[i]));
				}
			}
			loadThumbnailFromDiskCacheDependency.textureURL = text;
			loadThumbnailFromDiskCacheDependency.thumbnailType = typeToLoad;
			ILoadThumbnailTextureFromDiskCache loadThumbnailTextureFromDiskCache = serviceFactory.Create<ILoadThumbnailTextureFromDiskCache, LoadThumbnailFromDiskCacheDependency>(loadThumbnailFromDiskCacheDependency);
			loadThumbnailTextureFromDiskCache.SetAnswer(new ServiceAnswer<Texture2D>(onTextureLoaded, delegate(ServiceBehaviour behaviour)
			{
				OnCacheFailed(url, onTextureLoaded, behaviour, typeToLoad);
			}));
			loadThumbnailTextureFromDiskCache.Execute();
		}

		public void LoadTextureFromWeb(string url, Action<Texture2D> onTextureLoaded)
		{
			ILoadTextureFromURLRequest loadTextureFromURLRequest = serviceFactory.Create<ILoadTextureFromURLRequest, string>(url);
			loadTextureFromURLRequest.SetAnswer(new ServiceAnswer<Texture2D>(delegate(Texture2D texture)
			{
				OnTextureLoadedFromWeb(url, onTextureLoaded, texture);
			}));
			loadTextureFromURLRequest.Execute();
		}

		private void OnCacheFailed(string url, Action<Texture2D> onTextureLoaded, ServiceBehaviour behaviour, ThumbnailType typeToLoad)
		{
			if (typeToLoad != 0)
			{
				LoadTextureFromWeb(url, onTextureLoaded);
			}
		}

		private void OnTextureLoadedFromWeb(string url, Action<Texture2D> onTextureLoaded, Texture2D texture, ThumbnailType typeToLoad = ThumbnailType.Shop)
		{
			SaveTextureToDiskCacheDependency saveTextureToDiskCacheDependency = new SaveTextureToDiskCacheDependency();
			string[] array = url.Split('/');
			string text = string.Empty;
			for (int i = 0; i < array.Length - 1; i++)
			{
				text = ((i != array.Length - 2) ? (text + array[i] + "/") : (text + array[i]));
			}
			saveTextureToDiskCacheDependency.textureURL = text;
			saveTextureToDiskCacheDependency.texture = texture;
			saveTextureToDiskCacheDependency.thumbnailType = typeToLoad;
			ISaveThumbnailTextureToDiskCache saveThumbnailTextureToDiskCache = serviceFactory.Create<ISaveThumbnailTextureToDiskCache, SaveTextureToDiskCacheDependency>(saveTextureToDiskCacheDependency);
			saveThumbnailTextureToDiskCache.Execute();
			onTextureLoaded(texture);
		}
	}
}
