using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogAskedToReconnectRequest : IServiceRequest, IAnswerOnComplete
	{
	}
}
