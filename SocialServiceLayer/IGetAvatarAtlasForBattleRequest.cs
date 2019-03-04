using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface IGetAvatarAtlasForBattleRequest : IServiceRequest<GetAvatarAtlasRequestDependancy>, IAnswerOnComplete<AvatarAtlasForBattle>, ITask, IServiceRequest, IAbstractTask
	{
	}
}
