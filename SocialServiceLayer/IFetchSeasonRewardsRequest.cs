using SocialServiceLayer.Photon;
using Svelto.ServiceLayer;

namespace SocialServiceLayer
{
	internal interface IFetchSeasonRewardsRequest : IServiceRequest<FetchSeasonRewardsDependancy>, IAnswerOnComplete<FetchSeasonRewardsResponse>, IServiceRequest
	{
	}
}
