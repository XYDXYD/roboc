using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerXpEarnedRequest : IServiceRequest<LogPlayerXpEarnedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
