using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IChangeClanMemberRankRequest : IServiceRequest<ChangeClanMemberRankDependency>, IAnswerOnComplete<ClanMember[]>, IServiceRequest
	{
	}
}
