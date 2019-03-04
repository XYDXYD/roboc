using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface ILeaveChatChannelRequest : IServiceRequest<ChannelNameAndType>, IAnswerOnComplete, IServiceRequest
	{
	}
}
