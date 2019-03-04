using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogCollectedSeasonRewardRequest : IServiceRequest<int>, IAnswerOnComplete, IServiceRequest
	{
	}
}
