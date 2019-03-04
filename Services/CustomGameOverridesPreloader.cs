using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using WebServices;

namespace Services
{
	internal static class CustomGameOverridesPreloader
	{
		public static IEnumerator PreloadCustomGameOverrides(LoadingIconPresenter loadingIcon)
		{
			WebStorageRequestFactoryCustomGame requestFactory = new WebStorageRequestFactoryCustomGame();
			IGetAutoRegenHealthSettings autoRegenRequest = requestFactory.Create<IGetAutoRegenHealthSettings>();
			ILoadBattleArenaSettingsRequest loadBattleArenaRequest = requestFactory.Create<ILoadBattleArenaSettingsRequest>();
			ILoadCubeListRequest cubelistRequest = requestFactory.Create<ILoadCubeListRequest>();
			IGetPowerBarSettingsRequest getPowerBarRequest = requestFactory.Create<IGetPowerBarSettingsRequest>();
			cubelistRequest.ClearCache();
			autoRegenRequest.ClearCache();
			loadBattleArenaRequest.ClearCache();
			getPowerBarRequest.ClearCache();
			TaskService<BattleArenaSettingsDependency> task7 = new TaskService<BattleArenaSettingsDependency>(loadBattleArenaRequest);
			TaskService<AutoRegenHealthSettingsData> task6 = new TaskService<AutoRegenHealthSettingsData>(autoRegenRequest);
			TaskService<ReadOnlyDictionary<CubeTypeID, CubeListData>> task5 = new TaskService<ReadOnlyDictionary<CubeTypeID, CubeListData>>(cubelistRequest);
			TaskService<PowerBarSettingsData> task4 = new TaskService<PowerBarSettingsData>(getPowerBarRequest);
			yield return (object)new ParallelTaskCollection(new IEnumerator[4]
			{
				new HandleTaskServiceWithError(task7, delegate
				{
					loadingIcon.NotifyLoading("CustomGameOverrides");
				}, delegate
				{
					loadingIcon.NotifyLoadingDone("CustomGameOverrides");
				}).GetEnumerator(),
				new HandleTaskServiceWithError(task6, delegate
				{
					loadingIcon.NotifyLoading("CustomGameOverrides");
				}, delegate
				{
					loadingIcon.NotifyLoadingDone("CustomGameOverrides");
				}).GetEnumerator(),
				new HandleTaskServiceWithError(task5, delegate
				{
					loadingIcon.NotifyLoading("CustomGameOverrides");
				}, delegate
				{
					loadingIcon.NotifyLoadingDone("CustomGameOverrides");
				}).GetEnumerator(),
				new HandleTaskServiceWithError(task4, delegate
				{
					loadingIcon.NotifyLoading("CustomGameOverrides");
				}, delegate
				{
					loadingIcon.NotifyLoadingDone("CustomGameOverrides");
				}).GetEnumerator()
			});
			yield return task7.succeeded && task6.succeeded && task5.succeeded && task4.succeeded;
		}
	}
}
