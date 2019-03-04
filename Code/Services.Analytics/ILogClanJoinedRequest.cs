using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogClanJoinedRequest : IServiceRequest<string>, IAnswerOnComplete, IServiceRequest
	{
	}
}
