using ServerStateServiceLayer;
using ServerStateServiceLayer.Requests.Photon;
using Services.Requests.Interfaces;
using Services.Web.Photon;
using Svelto.ServiceLayer;

namespace Login
{
	public class LoginWebservicesRequestFactory : ServiceRequestFactory, ILoginWebservicesRequestFactory, IServiceRequestFactory
	{
		public LoginWebservicesRequestFactory()
		{
			AddRelation<IGetMaintenanceModeRequest, GetMaintenanceModeRequest>();
			AddRelation<ILoadQualiltyLevelDataRequest, LoadQualiltyLevelDataRequest>();
			AddRelation<IValidateUserRequest, ValidateUserRequest>();
			AddRelation<ICheckGameVersionRequest, CheckGameVersionRequest>();
			AddRelation<ILoadTutorialStatusRequest, LoadTutorialStatusRequest>();
			AddRelation<IUpdateTutorialStatusRequest, UpdateTutorialStatusRequest>();
			AddRelation<ILoadSignupDate, LoadSignupDate>();
			AddRelation<IGetGameClientSettingsRequest, GetGameClientSettingsRequest>();
			AddRelation<IValidateGaragesRequest, ValidateGaragesRequest>();
			AddRelation<IValidatePlayerLevelRequest, ValidatePlayerLevelRequest>();
			AddRelation<ILoadPlatformConfigurationRequest, LoadPlatformConfigurationRequest>();
		}
	}
}
