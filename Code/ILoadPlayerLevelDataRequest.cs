using Svelto.ServiceLayer;
using System.Collections.Generic;

internal interface ILoadPlayerLevelDataRequest : IServiceRequest, IAnswerOnComplete<IDictionary<uint, uint>>
{
}
