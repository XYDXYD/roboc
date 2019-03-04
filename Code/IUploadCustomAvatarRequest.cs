using Svelto.ServiceLayer;

internal interface IUploadCustomAvatarRequest : IServiceRequest<byte[]>, IAnswerOnComplete, IServiceRequest
{
}
