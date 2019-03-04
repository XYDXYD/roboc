using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerDismantledRobotRequest : IServiceRequest<DismantledRobotDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
