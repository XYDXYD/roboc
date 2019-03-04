using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerCampaignWaveSummaryRequest : IServiceRequest<LogPlayerCampaignWaveSummaryDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
