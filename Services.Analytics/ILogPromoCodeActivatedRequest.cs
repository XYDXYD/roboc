using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPromoCodeActivatedRequest : IServiceRequest<LogPromoCodeActivatedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
