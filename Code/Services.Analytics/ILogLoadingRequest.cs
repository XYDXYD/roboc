using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogLoadingRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
