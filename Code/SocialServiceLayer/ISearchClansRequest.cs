using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface ISearchClansRequest : IServiceRequest<SearchClanDependency>, IAnswerOnComplete<ClanInfo[]>, IServiceRequest
	{
	}
}
