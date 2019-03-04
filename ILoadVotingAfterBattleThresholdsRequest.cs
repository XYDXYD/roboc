using Simulation.GUI;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System.Collections.Generic;

internal interface ILoadVotingAfterBattleThresholdsRequest : IServiceRequest, IAnswerOnComplete<Dictionary<VoteType, ThresholdData[]>>, ITask, IAbstractTask
{
}
