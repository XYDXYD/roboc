using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogQuestAddedRequest : IServiceRequest<LogQuestAddedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
