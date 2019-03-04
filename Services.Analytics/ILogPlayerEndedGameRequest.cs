using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerEndedGameRequest : IServiceRequest<LogPlayerEndedGameDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
