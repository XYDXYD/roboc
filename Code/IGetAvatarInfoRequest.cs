using Svelto.ServiceLayer;
using Svelto.Tasks;

internal interface IGetAvatarInfoRequest : IServiceRequest, IAnswerOnComplete<AvatarInfo>, ITask, IAbstractTask
{
}
