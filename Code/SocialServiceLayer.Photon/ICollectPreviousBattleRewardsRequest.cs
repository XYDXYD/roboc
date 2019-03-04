using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal interface ICollectPreviousBattleRewardsRequest : IServiceRequest<string>, IAnswerOnComplete<bool>, IServiceRequest
	{
	}
}
