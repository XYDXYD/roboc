using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogSuccessfulLoginRequest : IServiceRequest<LogSuccessfulLoginDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
