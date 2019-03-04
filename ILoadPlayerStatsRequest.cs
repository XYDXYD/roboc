using Svelto.ServiceLayer;
using System.Collections.Generic;

internal interface ILoadPlayerStatsRequest : IServiceRequest, IAnswerOnComplete<Dictionary<string, List<string>>>
{
}
