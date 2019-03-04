using Svelto.ServiceLayer;
using System.Collections.Generic;

internal interface ILoadHintProgressRequest : IServiceRequest, IAnswerOnComplete<List<byte>>
{
}
