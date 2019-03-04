using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogTierRankUpRequest : IServiceRequest<LogTierRankUpDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
