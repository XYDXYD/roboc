using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogRobotDownloadedRequest : IServiceRequest<LogRobotDownloadedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
