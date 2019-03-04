using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogClaimedPromotionRequest : IServiceRequest<LogClaimedPromotionDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
