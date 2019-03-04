using Svelto.ServiceLayer;
using System.Collections.Generic;

internal interface ISetNewInventoryCubesRequest : IServiceRequest<HashSet<uint>>, IAnswerOnComplete, IServiceRequest
{
}
