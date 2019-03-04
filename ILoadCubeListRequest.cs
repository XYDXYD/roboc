using Svelto.DataStructures;
using Svelto.ServiceLayer;

internal interface ILoadCubeListRequest : IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<CubeTypeID, CubeListData>>
{
	void ClearCache();

	void SetParameterOverride(ParameterOverride parameterOverride);
}
