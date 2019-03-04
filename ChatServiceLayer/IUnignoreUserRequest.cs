using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IUnignoreUserRequest : IServiceRequest<string>, IAnswerOnComplete<IgnoreUserResponse>, IServiceRequest
	{
	}
}
