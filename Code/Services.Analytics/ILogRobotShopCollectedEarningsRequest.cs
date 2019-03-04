using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogRobotShopCollectedEarningsRequest : IServiceRequest<LogRobotShopCollectedEarningsDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
