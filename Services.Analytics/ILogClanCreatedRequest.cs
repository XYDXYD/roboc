using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogClanCreatedRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
