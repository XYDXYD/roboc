using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface IGetMyClanInfoAndMembersRequest : IServiceRequest, IAnswerOnComplete<ClanInfoAndMembers>, ITask, IAbstractTask
	{
		bool ForceRefresh
		{
			get;
			set;
		}
	}
}
