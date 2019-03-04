using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface ISendPrivateMessageRequest : IServiceRequest<PrivateMessageDependency>, IAnswerOnComplete, IServiceRequest
	{
	}
}
