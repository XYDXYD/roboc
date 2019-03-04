using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogChatSentRequest : IServiceRequest<LogChatSentDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
