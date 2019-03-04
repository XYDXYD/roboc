using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface IGetClanInfoAndMembersRequest : IServiceRequest<string>, IAnswerOnComplete<ClanInfoAndMembers>, ITask, IServiceRequest, IAbstractTask
	{
	}
}
