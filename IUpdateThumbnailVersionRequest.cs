using Svelto.ServiceLayer;

internal interface IUpdateThumbnailVersionRequest : IServiceRequest<UpdateThumbnailVersionRequestDependancy>, IAnswerOnComplete, IServiceRequest
{
}
