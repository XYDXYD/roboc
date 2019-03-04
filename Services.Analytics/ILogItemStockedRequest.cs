using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogItemStockedRequest : IServiceRequest<LogItemStockedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
