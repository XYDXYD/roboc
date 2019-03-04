using Svelto.DataStructures;
using Svelto.ServiceLayer;

internal interface ILoadInitialCubeInventoryRequest : IServiceRequest, IAnswerOnComplete<ReadOnlyDictionary<CubeTypeID, uint>>
{
}
