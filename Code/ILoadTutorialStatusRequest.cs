using Svelto.ServiceLayer;

internal interface ILoadTutorialStatusRequest : IServiceRequest, IAnswerOnComplete<LoadTutorialStatusData>
{
}
