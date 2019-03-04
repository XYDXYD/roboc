using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IInviteToClanRequest : IServiceRequest<string>, IAnswerOnComplete<ClanMember[]>, IServiceRequest
	{
	}
}
