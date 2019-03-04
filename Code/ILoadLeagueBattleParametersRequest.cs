using Svelto.ServiceLayer;
using Svelto.Tasks;

internal interface ILoadLeagueBattleParametersRequest : IServiceRequest, IAnswerOnComplete<uint[]>, ITask, IAbstractTask
{
}
