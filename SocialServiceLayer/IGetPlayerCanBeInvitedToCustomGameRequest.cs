using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IGetPlayerCanBeInvitedToCustomGameRequest : IServiceRequest<string>, IAnswerOnComplete<bool>, IServiceRequest
	{
	}
}
