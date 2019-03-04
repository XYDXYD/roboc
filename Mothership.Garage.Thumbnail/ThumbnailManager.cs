using Authentication;
using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Mothership.Garage.Thumbnail
{
	internal class ThumbnailManager : IInitialize, IWaitForFrameworkDestruction
	{
		private Action<UniqueSlotIdentifier, Texture2D> _thumbnailReadyCallback;

		private static StringBuilder sb = new StringBuilder();

		private ThumbnailCreator _thumbnailCreator;

		[Inject]
		internal RobotThumbnailFetcher thumbnailsFetcher
		{
			private get;
			set;
		}

		[Inject]
		internal ThumbnailTriggerer thumbnailtriggerer
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

		public static string GetGarageThumbnailFileName(string uniqueID)
		{
			HashAlgorithm hashAlgorithm = new SHA1CryptoServiceProvider();
			string username = User.Username;
			byte[] bytes = Encoding.UTF8.GetBytes(username);
			byte[] array = hashAlgorithm.ComputeHash(bytes);
			sb.Length = 0;
			sb.Append("garage_");
			for (int i = 0; i < 4; i++)
			{
				sb.Append(array[i]);
			}
			return sb.Append("_").Append(uniqueID).Append(".png")
				.ToString();
		}

		void IInitialize.OnDependenciesInjected()
		{
			thumbnailtriggerer.OnNewThumbnailNeeded += HandleNewThumbnailIsNeeded;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			thumbnailtriggerer.OnNewThumbnailNeeded -= HandleNewThumbnailIsNeeded;
		}

		public void SetupThumbnailCreator(ThumbnailCreator thumbnailCreator)
		{
			_thumbnailCreator = thumbnailCreator;
		}

		public void SetThumbnailReadyCallback(Action<UniqueSlotIdentifier, Texture2D> callbackWhenTextureReady)
		{
			_thumbnailReadyCallback = callbackWhenTextureReady;
		}

		public void RequestThumbnailTexture(UniqueSlotIdentifier uniqueSlotID)
		{
			thumbnailtriggerer.TestLocalVersionOutdated(uniqueSlotID, OnLocalVersionOutdatedTestCallback);
		}

		private void OnLocalVersionOutdatedTestCallback(UniqueSlotIdentifier uniqueSlotID, bool isOutdated)
		{
			if (!isOutdated)
			{
				string garageThumbnailFileName = GetGarageThumbnailFileName(uniqueSlotID.ToString());
				thumbnailsFetcher.LoadTexture(garageThumbnailFileName, delegate(Texture2D texture)
				{
					HandleThumbnailSuccessfullyFetched(uniqueSlotID, texture);
				}, ThumbnailType.Garage);
			}
		}

		private void HandleThumbnailSuccessfullyFetched(UniqueSlotIdentifier uniqueSlotID, Texture2D texture)
		{
			_thumbnailReadyCallback(uniqueSlotID, texture);
		}

		private void HandleNewThumbnailIsNeeded(UniqueSlotIdentifier slotIdentifier, uint slotId, uint remoteVersionNumber)
		{
			TaskRunner.get_Instance().Run(UpdateThumbnail(slotIdentifier, slotId, remoteVersionNumber));
		}

		public IEnumerator UpdateGarageThumbnail(uint slotIndex)
		{
			UniqueSlotIdentifier slotId = thumbnailtriggerer.GetGarageSlotUniqueIdentifier(slotIndex);
			uint remoteVersionNumber = 0u;
			thumbnailtriggerer.GetGarageSlotRemoteVersionNumber(slotId, out remoteVersionNumber);
			LoadRobotHashFromDiskCacheDependency dependency = new LoadRobotHashFromDiskCacheDependency
			{
				uniqueSlotIdentifier = slotId
			};
			TaskService<uint> cacheTask = new TaskService<uint>(serviceFactory.Create<ILoadRobotHashFromDiskCache, LoadRobotHashFromDiskCacheDependency>(dependency));
			yield return (object)new TaskWrapper(cacheTask);
			uint newVersion;
			if (cacheTask.behaviour == null)
			{
				uint result = cacheTask.result;
				newVersion = ((remoteVersionNumber <= result) ? result : remoteVersionNumber);
			}
			else
			{
				newVersion = 1u;
			}
			yield return UpdateThumbnail(dependency.uniqueSlotIdentifier, slotIndex, newVersion);
		}

		private IEnumerator UpdateThumbnail(UniqueSlotIdentifier slotIdentifier, uint slotId, uint remoteVersionNumber)
		{
			yield return _thumbnailCreator.RenderGarageThumbnail(highQuality: true, delegate(Texture2D texture2d)
			{
				SaveThumbnailToDisk(slotIdentifier, slotId, remoteVersionNumber, texture2d);
			});
		}

		private void SaveThumbnailToDisk(UniqueSlotIdentifier slotIdentifier, uint slotId, uint remoteVersionNumber, Texture2D texture2d)
		{
			SaveTextureToDiskCacheDependency saveTextureToDiskCacheDependency = new SaveTextureToDiskCacheDependency();
			saveTextureToDiskCacheDependency.thumbnailType = ThumbnailType.Garage;
			saveTextureToDiskCacheDependency.textureURL = GetGarageThumbnailFileName(slotIdentifier.ToString());
			saveTextureToDiskCacheDependency.texture = texture2d;
			_thumbnailReadyCallback(slotIdentifier, saveTextureToDiskCacheDependency.texture);
			ISaveThumbnailTextureToDiskCache saveThumbnailTextureToDiskCache = serviceFactory.Create<ISaveThumbnailTextureToDiskCache, SaveTextureToDiskCacheDependency>(saveTextureToDiskCacheDependency);
			saveThumbnailTextureToDiskCache.Execute();
			SaveRobotHashToDiskCacheDependency saveRobotHashToDiskCacheDependency = new SaveRobotHashToDiskCacheDependency();
			saveRobotHashToDiskCacheDependency.uniqueIdToIncrement = slotIdentifier;
			saveRobotHashToDiskCacheDependency.versionNumberToSetIfAny = remoteVersionNumber;
			ISaveRobotHashToDiskCache saveRobotHashToDiskCache = serviceFactory.Create<ISaveRobotHashToDiskCache, SaveRobotHashToDiskCacheDependency>(saveRobotHashToDiskCacheDependency);
			saveRobotHashToDiskCache.SetAnswer(new ServiceAnswer<uint>(delegate(uint newVersionNumber)
			{
				UpdateThumbnailVersionRequestDependancy param = new UpdateThumbnailVersionRequestDependancy(slotIdentifier, slotId, newVersionNumber);
				IUpdateThumbnailVersionRequest updateThumbnailVersionRequest = serviceFactory.Create<IUpdateThumbnailVersionRequest, UpdateThumbnailVersionRequestDependancy>(param);
				updateThumbnailVersionRequest.SetAnswer(new ServiceAnswer((Action<ServiceBehaviour>)delegate
				{
				}));
				updateThumbnailVersionRequest.Execute();
			}, delegate
			{
			}));
			saveRobotHashToDiskCache.Execute();
		}
	}
}
