using Services.Web;
using Services.Web.Photon;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using Utility;
using WebServices;

namespace Services
{
	internal static class BrawlOverridePreloader
	{
		internal class VersionCheckResult
		{
			public bool versionChanged;

			public ServiceBehaviour failBehaviour;
		}

		public static IEnumerator PreloadBrawlOverrides(bool clearCache, LoadingIconPresenter loadingIcon)
		{
			loadingIcon.NotifyLoading("BrawlOverrides");
			WebStorageRequestFactoryBrawl requestFactory = new WebStorageRequestFactoryBrawl();
			ILoadCubeListRequest cubelistRequest = requestFactory.Create<ILoadCubeListRequest>();
			ILoadWeaponStatsRequest weaponStatsRequest = requestFactory.Create<ILoadWeaponStatsRequest>();
			IGetPowerBarSettingsRequest powerbarRequest = requestFactory.Create<IGetPowerBarSettingsRequest>();
			IGetAutoRegenHealthSettings autoRegenRequest = requestFactory.Create<IGetAutoRegenHealthSettings>();
			if (clearCache)
			{
				cubelistRequest.ClearCache();
				weaponStatsRequest.ClearCache();
				powerbarRequest.ClearCache();
				autoRegenRequest.ClearCache();
			}
			TaskService<ReadOnlyDictionary<CubeTypeID, CubeListData>> task7 = new TaskService<ReadOnlyDictionary<CubeTypeID, CubeListData>>(cubelistRequest);
			TaskService<IDictionary<int, WeaponStatsData>> task6 = new TaskService<IDictionary<int, WeaponStatsData>>(weaponStatsRequest);
			TaskService<PowerBarSettingsData> task5 = new TaskService<PowerBarSettingsData>(powerbarRequest);
			TaskService<AutoRegenHealthSettingsData> task4 = new TaskService<AutoRegenHealthSettingsData>(autoRegenRequest);
			yield return (object)new ParallelTaskCollection(new IEnumerator[4]
			{
				new HandleTaskServiceWithError(task7, delegate
				{
					loadingIcon.NotifyLoading("BrawlOverrides");
				}, delegate
				{
					loadingIcon.NotifyLoadingDone("BrawlOverrides");
				}).GetEnumerator(),
				new HandleTaskServiceWithError(task6, delegate
				{
					loadingIcon.NotifyLoading("BrawlOverrides");
				}, delegate
				{
					loadingIcon.NotifyLoadingDone("BrawlOverrides");
				}).GetEnumerator(),
				new HandleTaskServiceWithError(task5, delegate
				{
					loadingIcon.NotifyLoading("BrawlOverrides");
				}, delegate
				{
					loadingIcon.NotifyLoadingDone("BrawlOverrides");
				}).GetEnumerator(),
				new HandleTaskServiceWithError(task4, delegate
				{
					loadingIcon.NotifyLoading("BrawlOverrides");
				}, delegate
				{
					loadingIcon.NotifyLoadingDone("BrawlOverrides");
				}).GetEnumerator()
			});
			yield return task7.succeeded && task6.succeeded && task5.succeeded && task4.succeeded;
			loadingIcon.NotifyLoadingDone("BrawlOverrides");
		}

		public static IEnumerator LoadBrawlLanguageStringOverrides(IServiceRequestFactory serviceFactory, LoadingIconPresenter loadingIcon)
		{
			loadingIcon.NotifyLoading("BrawlLanguageStrings");
			ILoadBrawlLanguageOverrides loadBrawlLanguageOverride = serviceFactory.Create<ILoadBrawlLanguageOverrides>();
			loadBrawlLanguageOverride.ClearCache();
			loadBrawlLanguageOverride.Inject(Localization.get_language());
			TaskService<BrawlOverrideLanguageStrings> taskService = new TaskService<BrawlOverrideLanguageStrings>(loadBrawlLanguageOverride);
			yield return new HandleTaskServiceWithError(taskService, delegate
			{
				loadingIcon.NotifyLoading("BrawlLanguageStrings");
			}, delegate
			{
				loadingIcon.NotifyLoadingDone("BrawlLanguageStrings");
			}).GetEnumerator();
			if (taskService.succeeded)
			{
				BrawlOverrideLanguageStrings result = taskService.result;
				Console.Log("Loaded override language strings for " + Localization.get_language() + ": ");
				foreach (KeyValuePair<string, string> languageString in result.LanguageStrings)
				{
					Console.Log(languageString.Key + " = " + languageString.Value);
					Localization.Set(languageString.Key, languageString.Value);
				}
			}
			loadingIcon.NotifyLoadingDone("BrawlLanguageStrings");
		}

		public static IEnumerator CheckVersionChange(IServiceRequestFactory serviceFactory)
		{
			TaskService<GetBrawlRequestResult> task3 = new TaskService<GetBrawlRequestResult>(serviceFactory.Create<IGetBrawlParametersRequest>());
			yield return task3;
			if (task3.succeeded)
			{
				IGetBrawlParametersRequest request2 = serviceFactory.Create<IGetBrawlParametersRequest>();
				request2.ClearCache();
				TaskService<GetBrawlRequestResult> task2 = new TaskService<GetBrawlRequestResult>(request2);
				yield return task2;
				yield return new VersionCheckResult
				{
					failBehaviour = task2.behaviour,
					versionChanged = (task3.result.BrawlParameters.VersionNumber != task2.result.BrawlParameters.VersionNumber)
				};
			}
		}
	}
}
