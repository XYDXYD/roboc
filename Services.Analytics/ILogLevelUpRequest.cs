using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogLevelUpRequest : IServiceRequest<LogLevelUpDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
