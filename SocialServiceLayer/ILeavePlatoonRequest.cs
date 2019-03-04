using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface ILeavePlatoonRequest : IServiceRequest, IAnswerOnComplete, ITask, IAbstractTask
	{
	}
}
