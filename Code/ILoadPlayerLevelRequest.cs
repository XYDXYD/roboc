using Svelto.ServiceLayer;

internal interface ILoadPlayerLevelRequest : IServiceRequest, IAnswerOnComplete<uint[]>
{
}
