using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogFrameRateRequest : IServiceRequest<LogFrameRateDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
