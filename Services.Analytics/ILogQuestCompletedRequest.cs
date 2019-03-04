using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogQuestCompletedRequest : IServiceRequest<LogQuestCompletedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
