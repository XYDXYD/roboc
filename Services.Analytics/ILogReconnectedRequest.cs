using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogReconnectedRequest : IServiceRequest<float>, IAnswerOnComplete, IServiceRequest
	{
	}
}
