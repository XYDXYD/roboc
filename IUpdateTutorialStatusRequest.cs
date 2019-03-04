using Svelto.ServiceLayer;

internal interface IUpdateTutorialStatusRequest : IServiceRequest<UpdateTutorialStatusData>, IAnswerOnComplete, IServiceRequest
{
}
