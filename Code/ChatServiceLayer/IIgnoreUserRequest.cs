using Svelto.ServiceLayer;

namespace ChatServiceLayer
{
	internal interface IIgnoreUserRequest : IServiceRequest<string>, IAnswerOnComplete<IgnoreUserResponse>, IServiceRequest
	{
	}
}
