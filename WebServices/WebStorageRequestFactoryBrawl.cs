using Services.Web.Photon;

namespace WebServices
{
	internal class WebStorageRequestFactoryBrawl : WebStorageRequestFactory
	{
		public WebStorageRequestFactoryBrawl()
		{
			AddRelation<ILoadCubeListRequest, GetBrawlCubeListRequest>();
			AddRelation<ILoadWeaponStatsRequest, GetBrawlWeaponStatsRequest>();
			AddRelation<IGetPowerBarSettingsRequest, GetBrawlPowerBarSettingsRequest>();
			AddRelation<IGetAutoRegenHealthSettings, GetBrawlAutoRegenHealthSettingsRequest>();
		}
	}
}
