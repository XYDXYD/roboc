using Svelto.ServiceLayer;

internal interface ISaveRobotHashToDiskCache : IServiceRequest<SaveRobotHashToDiskCacheDependency>, IAnswerOnComplete<uint>, IServiceRequest
{
}
