using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IRemoveFromClanRequest : IServiceRequest<string>, IAnswerOnComplete<ClanMember[]>, IServiceRequest
	{
	}
}
