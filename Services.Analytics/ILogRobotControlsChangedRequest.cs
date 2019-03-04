using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogRobotControlsChangedRequest : IServiceRequest<LogRobotControlsChangedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
