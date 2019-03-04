using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface IGetGameModePreferencesRequest : IServiceRequest, IAnswerOnComplete<GameModePreferences>
	{
		void ClearCache();
	}
}
