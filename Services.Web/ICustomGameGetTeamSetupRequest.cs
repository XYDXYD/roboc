using Svelto.ServiceLayer;

namespace Services.Web
{
	internal interface ICustomGameGetTeamSetupRequest : IServiceRequest<GameModeType>, IAnswerOnComplete<int>, IServiceRequest
	{
	}
}
