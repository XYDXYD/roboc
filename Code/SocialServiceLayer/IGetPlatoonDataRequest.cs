using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace SocialServiceLayer
{
	internal interface IGetPlatoonDataRequest : IServiceRequest, IAnswerOnComplete<Platoon>, ITask, IAbstractTask
	{
		void ForceRefresh();
	}
}
