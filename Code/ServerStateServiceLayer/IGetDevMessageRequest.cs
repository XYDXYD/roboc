using Svelto.ServiceLayer;

namespace ServerStateServiceLayer
{
	internal interface IGetDevMessageRequest : IServiceRequest, IAnswerOnComplete<DevMessage>
	{
	}
}
