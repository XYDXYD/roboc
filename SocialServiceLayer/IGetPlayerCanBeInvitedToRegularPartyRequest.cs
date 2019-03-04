using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IGetPlayerCanBeInvitedToRegularPartyRequest : IServiceRequest<string>, IAnswerOnComplete<GetPlayerCanBeInvitedToRegularPartyResponseCode>, IServiceRequest
	{
	}
}
