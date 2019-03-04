using Svelto.ServiceLayer;

internal interface ISetAvatarInfoRequest : IServiceRequest<AvatarInfo>, IAnswerOnComplete, IServiceRequest
{
}
