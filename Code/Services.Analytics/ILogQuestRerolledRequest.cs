using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogQuestRerolledRequest : IServiceRequest<LogQuestRerolledDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
