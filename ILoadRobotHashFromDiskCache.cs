using Svelto.ServiceLayer;

internal interface ILoadRobotHashFromDiskCache : IServiceRequest<LoadRobotHashFromDiskCacheDependency>, IAnswerOnComplete<uint>, IServiceRequest
{
}
