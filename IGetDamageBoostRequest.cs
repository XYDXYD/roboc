using Svelto.ServiceLayer;
using Svelto.Tasks;

internal interface IGetDamageBoostRequest : IServiceRequest, IAnswerOnComplete<DamageBoostDeserialisedData>, ITask, IAbstractTask
{
	void ClearCache();
}
