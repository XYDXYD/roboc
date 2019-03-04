using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace Services.Web.Photon
{
	internal interface IGetBrawlParametersRequest : IServiceRequest, IAnswerOnComplete<GetBrawlRequestResult>, ITask, IAbstractTask
	{
		void ClearCache();
	}
}
