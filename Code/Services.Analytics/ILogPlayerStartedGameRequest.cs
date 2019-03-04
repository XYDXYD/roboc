using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerStartedGameRequest : IServiceRequest<LogPlayerStartedGameDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
