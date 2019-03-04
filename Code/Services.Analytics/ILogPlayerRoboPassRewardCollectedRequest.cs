using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerRoboPassRewardCollectedRequest : IServiceRequest<LogPlayerRoboPassRewardCollectedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
