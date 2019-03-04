using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPurchaseFunnelRequest : IServiceRequest<LogPurchaseFunnelDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
