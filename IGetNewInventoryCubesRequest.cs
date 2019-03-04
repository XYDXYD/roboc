using Svelto.ServiceLayer;
using System.Collections.Generic;

internal interface IGetNewInventoryCubesRequest : IServiceRequest, IAnswerOnComplete<HashSet<uint>>
{
}
