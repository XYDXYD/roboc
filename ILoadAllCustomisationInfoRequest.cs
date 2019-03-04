using Svelto.ServiceLayer;
using Svelto.Tasks;

internal interface ILoadAllCustomisationInfoRequest : IServiceRequest, IAnswerOnComplete<AllCustomisationsResponse>, ITask, IAbstractTask
{
	void ClearCache();
}
