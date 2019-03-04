using Svelto.ServiceLayer;
using System.Collections.Generic;

internal interface ILoadUserCubeInventoryRequest : IServiceRequest, IAnswerOnComplete<Dictionary<uint, uint>>
{
}
