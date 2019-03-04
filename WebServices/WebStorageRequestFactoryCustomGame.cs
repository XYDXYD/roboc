using Services.Web.Photon;

namespace WebServices
{
	internal class WebStorageRequestFactoryCustomGame : WebStorageRequestFactory
	{
		public WebStorageRequestFactoryCustomGame()
		{
			AddRelation<IGetAutoRegenHealthSettings, GetAutoRegenHealthSettingsCustomGame>();
			AddRelation<ILoadBattleArenaSettingsRequest, LoadBattleArenaSettingsCustomGameRequest>();
			AddRelation<ILoadCubeListRequest, LoadCubeListCustomGameRequest>();
			AddRelation<IGetPowerBarSettingsRequest, GetPowerBarSettingsCustomGameRequest>();
		}
	}
}
