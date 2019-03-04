using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface IPlatoonRobotTierChangeRequest : IServiceRequest<int>, IAnswerOnComplete, ITask, IServiceRequest, IAbstractTask
	{
	}
}
