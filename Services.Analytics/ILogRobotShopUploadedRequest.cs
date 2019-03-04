using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogRobotShopUploadedRequest : IServiceRequest<LogRobotShopUploadedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
