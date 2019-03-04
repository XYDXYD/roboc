using Svelto.ServiceLayer;
using Svelto.Tasks;

internal interface IGetTauntsRequest : IServiceRequest, IAnswerOnComplete<TauntsDeserialisedData>, ITask, IAbstractTask
{
	void ClearCache();
}
