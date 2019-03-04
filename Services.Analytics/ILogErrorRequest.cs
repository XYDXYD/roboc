using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogErrorRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
