using Services.Web.Photon;

namespace WebServices
{
	internal class WebStorageRequestFactoryCampaign : WebStorageRequestFactory
	{
		public WebStorageRequestFactoryCampaign()
		{
			AddRelation<ILoadCubeListRequest, LoadCampaignCubeListRequest>();
			AddRelation<ILoadWeaponStatsRequest, LoadCampaignWeaponStatsRequest>();
			AddRelation<IGetPowerBarSettingsRequest, LoadCampaignPowerBarSettingsRequest>();
			AddRelation<IGetAutoRegenHealthSettings, LoadCampaignAutoRegenHealthSettingsRequest>();
			AddRelation<ISaveCampaignGameAwardsRequest, SaveCampaignGameAwardsRequest, SaveGameAwardsRequestDependency>();
		}
	}
}
