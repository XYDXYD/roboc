using Services.Web.Photon;
using SinglePlayerCampaign.GUI.Mothership;
using Svelto.DataStructures;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections;
using System.Collections.Generic;
using WebServices;

internal static class CampaignJsonOverridesPreloader
{
	public static IEnumerator PreloadCampaignOverrides(bool clearCache, LoadingIconPresenter loadingIcon, SelectedCampaignToStart selectedCampaign)
	{
		loadingIcon.NotifyLoading("LoadingSinglePlayerCampaignOverrides");
		WebStorageRequestFactoryCampaign requestFactory = new WebStorageRequestFactoryCampaign();
		ParameterOverride campaignIDOverride = new ParameterOverride(WebServicesParameterCode.CampaignID, selectedCampaign.CampaignID);
		ILoadCubeListRequest cubelistRequest = requestFactory.Create<ILoadCubeListRequest>();
		cubelistRequest.SetParameterOverride(campaignIDOverride);
		ILoadWeaponStatsRequest weaponStatsRequest = requestFactory.Create<ILoadWeaponStatsRequest>();
		weaponStatsRequest.SetParameterOverride(campaignIDOverride);
		IGetPowerBarSettingsRequest powerbarRequest = requestFactory.Create<IGetPowerBarSettingsRequest>();
		powerbarRequest.SetParameterOverride(campaignIDOverride);
		IGetAutoRegenHealthSettings autoRegenRequest = requestFactory.Create<IGetAutoRegenHealthSettings>();
		autoRegenRequest.SetParameterOverrides(new ParameterOverride[3]
		{
			campaignIDOverride,
			new ParameterOverride(WebServicesParameterCode.CampaignDifficulty, selectedCampaign.Difficulty),
			new ParameterOverride(WebServicesParameterCode.IsCustomGameOverride, false)
		});
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
				loadingIcon.NotifyLoading("LoadingSinglePlayerCampaignOverrides");
			}, delegate
			{
				loadingIcon.NotifyLoadingDone("LoadingSinglePlayerCampaignOverrides");
			}).GetEnumerator(),
			new HandleTaskServiceWithError(task6, delegate
			{
				loadingIcon.NotifyLoading("LoadingSinglePlayerCampaignOverrides");
			}, delegate
			{
				loadingIcon.NotifyLoadingDone("LoadingSinglePlayerCampaignOverrides");
			}).GetEnumerator(),
			new HandleTaskServiceWithError(task5, delegate
			{
				loadingIcon.NotifyLoading("LoadingSinglePlayerCampaignOverrides");
			}, delegate
			{
				loadingIcon.NotifyLoadingDone("LoadingSinglePlayerCampaignOverrides");
			}).GetEnumerator(),
			new HandleTaskServiceWithError(task4, delegate
			{
				loadingIcon.NotifyLoading("LoadingSinglePlayerCampaignOverrides");
			}, delegate
			{
				loadingIcon.NotifyLoadingDone("LoadingSinglePlayerCampaignOverrides");
			}).GetEnumerator()
		});
		yield return task7.succeeded && task6.succeeded && task5.succeeded && task4.succeeded;
		loadingIcon.NotifyLoadingDone("LoadingSinglePlayerCampaignOverrides");
	}
}
