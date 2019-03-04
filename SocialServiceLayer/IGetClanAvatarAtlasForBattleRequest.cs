using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface IGetClanAvatarAtlasForBattleRequest : IServiceRequest<GetClanAvatarAtlasRequestDependancy>, IAnswerOnComplete<AvatarAtlasForBattle>, ITask, IServiceRequest, IAbstractTask
	{
	}
}
