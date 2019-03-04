using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogOnLoggedInPlayerDataRequest : IServiceRequest<LogOnLoggedInPlayerDataDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
