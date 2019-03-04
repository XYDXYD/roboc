using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogPlayerLeftMothershipRequest : IServiceRequest<LogPlayerLeftMothershipDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
