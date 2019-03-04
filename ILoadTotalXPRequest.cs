using Svelto.ServiceLayer;

internal interface ILoadTotalXPRequest : IServiceRequest, IAnswerOnComplete<uint[]>
{
}
