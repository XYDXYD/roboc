using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IGetCanSendPrivateMessageRequest : IServiceRequest<string>, IAnswerOnComplete<CanSendWhisperRequestResult>, IServiceRequest
	{
	}
}
