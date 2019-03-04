using Svelto.ServiceLayer;

namespace Services.Analytics
{
	public interface ILogPlayerKillRequest : IServiceRequest<LogPlayerKillDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
