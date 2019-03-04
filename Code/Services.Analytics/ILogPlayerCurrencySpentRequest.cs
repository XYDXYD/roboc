using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerCurrencySpentRequest : IServiceRequest<LogPlayerCurrencySpentDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
