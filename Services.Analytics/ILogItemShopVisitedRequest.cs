using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogItemShopVisitedRequest : IServiceRequest<LogItemShopVisitedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
