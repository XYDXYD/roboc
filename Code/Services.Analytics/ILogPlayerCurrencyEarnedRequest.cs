using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerCurrencyEarnedRequest : IServiceRequest<LogPlayerCurrencyEarnedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
