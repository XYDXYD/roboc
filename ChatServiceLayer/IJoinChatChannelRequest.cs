using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IJoinChatChannelRequest : IServiceRequest<CreateOrJoinChatChannelDependency>, IAnswerOnComplete<ChatChannelInfo>, IServiceRequest
	{
	}
}
