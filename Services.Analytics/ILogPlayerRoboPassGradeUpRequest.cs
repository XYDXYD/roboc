using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerRoboPassGradeUpRequest : IServiceRequest<LogPlayerRoboPassGradeUpDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
