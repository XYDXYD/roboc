using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogRobotShopDownloadedRequest : IServiceRequest<LogRobotShopDownloadedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
