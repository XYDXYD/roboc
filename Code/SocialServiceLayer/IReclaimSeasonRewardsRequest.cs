using SocialServiceLayer.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface IReclaimSeasonRewardsRequest : IServiceRequest<string>, IAnswerOnComplete<ReclaimSeasonRewardsResponse>, ITask, IServiceRequest, IAbstractTask
	{
	}
}
