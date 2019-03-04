using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal interface IHasNewPreviousBattleRewardsRequest : IServiceRequest<string>, IAnswerOnComplete<bool>, IServiceRequest
	{
	}
}
