using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerCreatedNewRobotRequest : IServiceRequest<CreatedNewRobotDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
