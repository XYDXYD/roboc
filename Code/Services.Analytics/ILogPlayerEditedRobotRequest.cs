using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerEditedRobotRequest : IServiceRequest<EditedRobotDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
