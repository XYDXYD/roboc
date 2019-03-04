using Svelto.ServiceLayer;

namespace Services.Analytics
{
	internal interface ILogChatJoinedRequest : IServiceRequest<ChatChannelType>, IAnswerOnComplete, IServiceRequest
	{
	}
}
