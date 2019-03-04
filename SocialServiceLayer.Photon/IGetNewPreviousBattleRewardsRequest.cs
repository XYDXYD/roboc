using Svelto.ServiceLayer;

namespace SocialServiceLayer.Photon
{
	internal interface IGetNewPreviousBattleRewardsRequest : IServiceRequest<string>, IAnswerOnComplete<GetNewPreviousBattleRequestData>, IServiceRequest
	{
	}
}
