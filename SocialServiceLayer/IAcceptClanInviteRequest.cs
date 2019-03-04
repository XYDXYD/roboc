using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IAcceptClanInviteRequest : IServiceRequest<string>, IAnswerOnComplete<ClanInfoAndMembers>, IServiceRequest
	{
	}
}
