using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

namespace SinglePlayerServiceLayer
{
	internal interface ISinglePlayerLoadTdmAiRobotsRequest : IServiceRequest<SinglePlayerLoadTDMAIParameters>, IAnswerOnComplete<Dictionary<string, PlayerDataDependency>>, ITask, IServiceRequest, IAbstractTask
	{
	}
}
