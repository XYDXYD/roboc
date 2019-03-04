using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

internal interface ILoadWeaponStatsRequest : IServiceRequest, IAnswerOnComplete<IDictionary<int, WeaponStatsData>>, ITask, IAbstractTask
{
	void ClearCache();

	void SetParameterOverride(ParameterOverride parameterOverride);
}
