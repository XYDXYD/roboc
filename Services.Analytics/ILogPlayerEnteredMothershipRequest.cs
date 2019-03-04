using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerEnteredMothershipRequest : IServiceRequest<LogPlayerEnteredMothershipDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
