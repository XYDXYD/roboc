using Svelto.ServiceLayer;

namespace Services.Web.Photon
{
	internal interface ISaveGameModePreferencesRequest : IServiceRequest<GameModePreferences>, IAnswerOnComplete, IServiceRequest
	{
	}
}
