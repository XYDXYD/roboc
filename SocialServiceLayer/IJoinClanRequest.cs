using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IJoinClanRequest : IServiceRequest<string>, IAnswerOnComplete<ClanInfoAndMembers>, IServiceRequest
	{
	}
}
