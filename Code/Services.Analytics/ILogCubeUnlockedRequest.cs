using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogCubeUnlockedRequest : IServiceRequest<LogCubeUnlockedDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
