using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace Services.Web.Photon
{
	internal interface ILoadFirstBrawlVictoryPendingRequest : IServiceRequest<int>, IAnswerOnComplete<bool>, ITask, IServiceRequest, IAbstractTask
	{
		void ClearCache();
	}
}
