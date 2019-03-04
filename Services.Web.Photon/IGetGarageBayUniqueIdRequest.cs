using Svelto.ServiceLayer;
using Svelto.Tasks;

namespace Services.Web.Photon
{
	internal interface IGetGarageBayUniqueIdRequest : IServiceRequest, IAnswerOnComplete<UniqueSlotIdentifier>, ITask, IAbstractTask
	{
		void ClearCache();
	}
}
