using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface ICreateChatChannelRequest : IServiceRequest<CreateOrJoinChatChannelDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
